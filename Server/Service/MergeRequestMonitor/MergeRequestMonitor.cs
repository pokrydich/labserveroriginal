namespace LabServer.Server.Service;

using System.Globalization;

using Microsoft.EntityFrameworkCore;

using LabServer.Server.Data;
using LabServer.Shared.Models.Uni;
using LabServer.Server.Models.Uni;

public class MergeRequestMonitor : BackgroundService
{
    private readonly IGitLab _gitLab;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IConfiguration _configuration;
    private TimeSpan _runPeriod;
    public MergeRequestMonitor(IGitLab gitLab, IServiceScopeFactory serviceScopeFactory,
                                IConfiguration configuration)
    {
        _gitLab = gitLab;
        _serviceScopeFactory = serviceScopeFactory;
        _configuration = configuration;

        var period = _configuration.GetSection("Services")
                        .GetSection(typeof(MergeRequestMonitor).Name)
                        .GetValue<System.String>("period", "00:05:00");
        if (!TimeSpan.TryParseExact(period, @"hh\:mm\:ss", CultureInfo.InvariantCulture, out _runPeriod))
            _runPeriod = new TimeSpan(0, 5, 0);
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

                var studentLabs = context.StudentLabs.Where(sl => sl.Status == StudentLabStatus.cInProgress
                                                            || sl.Status == StudentLabStatus.cOverdue).ToList();
                var mergeRequests = await _gitLab.GetMergeRequests();
                foreach (var mergeRequest in mergeRequests)
                {
                    var studnetLab = studentLabs.SingleOrDefault(sl => sl.GitLabProjectId == mergeRequest.TargetProjectId);
                    if (studnetLab == null)
                        continue;
                    if (mergeRequest.Author.Id != studnetLab.Student.GitLabUserId)
                    {
                        Console.WriteLine("someone has done something wrong (only student should create merge requests to his lab repo)");
                        continue;
                    }
                    if (studnetLab.LabSubmissions.Any(sls => sls.GitLabMergeRequestId == mergeRequest.Id))
                    {
                        continue; // this merge request is already added as submission
                    }

                    var newLabSubmission = new StudentLabSubmissionModel
                    {
                        StudentLabId = studnetLab.Id,
                        GitLabMergeRequestId = mergeRequest.Id,
                        GitLabMergeRequestIid = mergeRequest.Iid,
                        SubmittedDate = mergeRequest.CreatedAt.ToUniversalTime()
                    };

                    await context.StudentLabSubmissions.AddAsync(newLabSubmission);
                    try
                    {
                        await context.SaveChangesAsync();
                    }
                    catch (DbUpdateException)
                    {
                        throw new NotImplementedException("error inserting lab submission");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Something went wrong in MergeMonitor: {e}");
                continue;
            }
        }
    }
}