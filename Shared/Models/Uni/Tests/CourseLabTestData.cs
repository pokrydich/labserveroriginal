namespace LabServer.Shared.Models.Uni;

public class CourseLabTestData : DataModel
{
    public System.Int64 Id { get; set; }
    public System.Int64 TestId { get; set; }
    public System.Int64 CourseLabId { get; set; }
    public System.Boolean Activated { get; set; }

    public CourseLabData? CourseLab;
    public TestData? Test { get; set; }

    public override void Update(DataModel other)
    {
        CourseLabTestData update = other as CourseLabTestData ?? throw new NotImplementedException();
        Activated = update.Activated;
    }
}