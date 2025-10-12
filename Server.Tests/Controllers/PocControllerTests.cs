namespace LabServer.Server.Tests.Controllers;

using LabServer.Server.Controllers;
using LabServer.Shared.Models.TestAPI;

public class PocControllerTests
{
    private static void ResetState()
    {
        var field = typeof(PocController).GetField("_testResults", BindingFlags.Static | BindingFlags.NonPublic);
        var dictionary = field!.GetValue(null) as System.Collections.IDictionary;
        dictionary!.Clear();
    }

    [Fact]
    public async Task ScheduleAndGetResult_ReturnsStoredOutcome()
    {
        ResetState();
        var controller = new PocController();
        var scheduleRequest = new ScheduleTestRequestModel
        {
            MergeRequest = new MergeRequestData
            {
                Title = "вжух",
                SourceProjectId = 10,
                CommitHash = "abc"
            }
        };

        var scheduleResponse = await controller.ScheduleTest(scheduleRequest);
        Assert.True(scheduleResponse.Success);

        var result = await controller.GetTestResult(new GetTestResultRequestModel
        {
            SourceProjectId = 10,
            CommitHash = "abc"
        });

        Assert.True(result.TestCompleted);
        Assert.True(result.Success);
    }

    [Fact]
    public async Task GetResult_ReturnsIncomplete_WhenNoSubmission()
    {
        ResetState();
        var controller = new PocController();

        var result = await controller.GetTestResult(new GetTestResultRequestModel
        {
            SourceProjectId = 999,
            CommitHash = "missing"
        });

        Assert.False(result.TestCompleted);
    }
}
