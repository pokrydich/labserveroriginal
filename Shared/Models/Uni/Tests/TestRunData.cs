namespace LabServer.Shared.Models.Uni;

public enum TestRunState
{
    NotScheduled = 0,
    Scheduled,
    Completed
}

public class TestRunData : DataModel
{
    System.Int64 Id { get; set; }
    public System.Int64 CourseLabTestMappingId { get; set; }
    public System.Int64 StudentLabSubmissionId { get; set; }
    public TestRunState State { get; set; }
    public DateTime ScheduledDate { get; set; }
    public System.Boolean? Success { get; set; } // stores test run boolean result (when the test is in State.Completed)
    public System.String? Message { get; set; } // stores test result description message (for example, the reasons of failure) (State.Completed required)

    public override void Update(DataModel other)
    {
        TestRunData update = other as TestRunData ?? throw new NotImplementedException();
        State = update.State;
        ScheduledDate = update.ScheduledDate;
        Success = update.Success;
        Message = update.Message;
    }
}