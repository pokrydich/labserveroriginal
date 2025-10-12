namespace LabServer.Server.Service;

using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;

using Microsoft.EntityFrameworkCore;

using LabServer.Server.Data;
using LabServer.Server.Models.Uni;

using LabServer.Shared.Models.Uni;
using LabServer.Shared.Models.TestAPI;
using System.Text;

public class TestRunner : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IConfiguration _configuration;
    private TimeSpan _runPeriod;
    private readonly HttpClient _httpClient;
    public TestRunner(IServiceScopeFactory serviceScopeFactory, IConfiguration configuration)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _configuration = configuration;

        var period = _configuration.GetSection("Services")
                        .GetSection(typeof(TestRunner).Name)
                        .GetValue<System.String>("period", "00:05:00");
        if (!TimeSpan.TryParseExact(period, @"hh\:mm\:ss", CultureInfo.InvariantCulture, out _runPeriod))
            _runPeriod = new TimeSpan(0, 5, 0);

        // TODO: production
        var debugHandler = new HttpClientHandler();
        debugHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
        debugHandler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;

        _httpClient = new HttpClient(debugHandler);
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new PeriodicTimer(_runPeriod);
        while (true && await timer.WaitForNextTickAsync())
        {
            try
            {

                await using var scope = _serviceScopeFactory.CreateAsyncScope();
                var context = scope.ServiceProvider.GetRequiredService<LabsContext>();

                var labSubmissions = await context.StudentLabSubmissions.ToListAsync();
                foreach (var labSubmission in labSubmissions
                                    .Where(ls => ls.Status == StudentLabSubmissionStatus.cActive)
                                    .OrderBy(ls => ls.SubmittedDate))
                {
                    try
                    {
                        var mergeRequest = labSubmission.GitLabMergeRequest;
                        if (mergeRequest == null)
                        {
                            Console.WriteLine("merge reques is migging");
                            continue;
                        }

                        var assignedTests = labSubmission.StudentLab.GroupCourseLab.CourseLab.TestMapping.Where(m => m.Activated);

                        foreach (var assignedTest in assignedTests)
                        {
                            var existingTestRun = labSubmission.TestRuns.SingleOrDefault(tr => tr.CourseLabTestMappingId == assignedTest.Id);
                            if (existingTestRun != null && existingTestRun.State == TestRunState.Scheduled) // test was already scheduled. Check if result is ready.
                            {
                                HttpResponseMessage? resp;
                                try
                                {
                                    resp = await _httpClient.PostAsJsonAsync<GetTestResultRequestModel>($"{assignedTest.Test.TestServerUrl}/getresult", new GetTestResultRequestModel
                                    {
                                        SourceProjectId = mergeRequest.SourceProjectId,
                                        CommitHash = mergeRequest.CommitHash
                                    });
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine($"error sending http req (get result): {e}");
                                    continue;
                                }
                                if (resp.StatusCode != HttpStatusCode.OK)
                                {
                                    Console.WriteLine("error response from test server (get result req)");
                                    continue;
                                }
                                var result = await resp.Content.ReadFromJsonAsync<GetTestResultResponseModel>();
                                if (result.TestCompleted)
                                {
                                    existingTestRun.Success = result.Success;
                                    existingTestRun.Message = result.Message;
                                    existingTestRun.State = TestRunState.Scheduled;
                                    // TODO: send updates via SignalR
                                    await context.SaveChangesAsync();

                                    var apiResult = await mergeRequest.AddNote(result.Message);
                                    if (!apiResult.Ok)
                                    {
                                        Console.WriteLine($"Couldn't add note to merge request: {apiResult.Error.Message}");
                                    }
                                }
                            }
                            else // test was not scheduled. Schedule it.
                            {
                                // try to schedule
                                HttpResponseMessage? resp;
                                try
                                {
                                    resp = await _httpClient.PostAsJsonAsync<ScheduleTestRequestModel>($"{assignedTest.Test.TestServerUrl}/schedule", new ScheduleTestRequestModel
                                    {
                                        MergeRequest = new MergeRequestData
                                        {
                                            Title = mergeRequest.Title,
                                            SourceProjectId = mergeRequest.SourceProjectId,
                                            CommitHash = mergeRequest.CommitHash
                                        },
                                        RepoArchiveBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("TODO"))
                                    });
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine($"error sending http req (schedule): {e}");
                                    continue;
                                }
                                if (resp.StatusCode != HttpStatusCode.OK)
                                {
                                    Console.WriteLine("error response from test server (schedule req)");
                                    continue;
                                }
                                var scheduleResult = await resp.Content.ReadFromJsonAsync<ScheduleTestResponseModel>();

                                if (scheduleResult.Success)
                                {
                                    var newTestRun = new TestRunModel
                                    {
                                        StudentLabSubmissionId = labSubmission.Id,
                                        CourseLabTestMappingId = assignedTest.Id,
                                        State = TestRunState.Scheduled,
                                        ScheduledDate = DateTime.UtcNow
                                    };
                                    labSubmission.TestRuns.Add(newTestRun);
                                    await context.SaveChangesAsync();
                                }
                                else
                                {
                                    Console.WriteLine("test server didn't schedule lab submission");
                                }
                            }
                        }
                    }
                    catch (Exception e) // TODO: best exception handeling practices
                    {
                        Console.WriteLine($"something went wrong while processing lab submission '{labSubmission.Id}': {e}");
                        continue;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"something went wrong in TestRunnes: {e}");
                continue;
            }
        }
    }
}