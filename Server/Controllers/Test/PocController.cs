namespace LabServer.Server.Controllers;

using System.Collections.Concurrent;

using Microsoft.AspNetCore.Mvc;

using LabServer.Shared.Models.TestAPI;

[Route("test/[controller]")]
[ApiController]
public class PocController : Controller
{
    private static readonly ConcurrentDictionary<Tuple<System.Int64, System.String>, System.Boolean> _testResults
            = new ConcurrentDictionary<Tuple<long, string>, bool>();
    public PocController() { }

    [HttpPost("schedule")]
    public async Task<ScheduleTestResponseModel> ScheduleTest([FromBody] ScheduleTestRequestModel scheduleTestRequest)
    {
        // 'check' lab submission in a thread
        await Task.Run(() =>
        {
            var result = scheduleTestRequest.MergeRequest.Title.ToLower() == "вжух";
            _testResults.TryAdd(new Tuple<long, string>(
                scheduleTestRequest.MergeRequest.SourceProjectId,
                scheduleTestRequest.MergeRequest.CommitHash), result);

        });
        return new ScheduleTestResponseModel
        {
            Success = true
        };
    }

    [HttpPost("getresult")]
    public async Task<GetTestResultResponseModel> GetTestResult([FromBody] GetTestResultRequestModel getTestResultRequest)
    {
        System.Boolean testResult;
        System.Boolean testCompleted = true;
        var key = new Tuple<long, string>(
            getTestResultRequest.SourceProjectId,
            getTestResultRequest.CommitHash);
        if (!_testResults.Remove(key, out testResult))
            testCompleted = false;
        return new GetTestResultResponseModel
        {
            TestCompleted = testCompleted,
            Success = testResult,
            Message = testResult ? "Test was passed" : "Test was not passed"
        };
    }
}