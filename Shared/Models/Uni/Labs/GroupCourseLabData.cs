namespace LabServer.Shared.Models.Uni;

using System.Text.Json.Serialization;

using GitLab.Models.Group;

public class GroupCourseLabData : DataModel
{
    public System.Int64 Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? DeadlineDate { get; set; }
    public System.Int64 GroupCourseId { get; set; }
    public System.Int64 CourseLabId { get; set; }
    public CourseLabData? CourseLab { get; set; }
    public System.Int64? GitLabGroupId { get; set; }
    [JsonIgnore]
    public System.Boolean GitLabSynced => GitLabGroupId != null;
    public GitLabGroup? GitLabGroup { get; set; }
    public IList<StudentLabData>? StudentLabs { get; set; }

    public override void Update(DataModel other)
    {
        GroupCourseLabData update = other as GroupCourseLabData ?? throw new NotImplementedException();
        DeadlineDate = update.DeadlineDate;
    }
}