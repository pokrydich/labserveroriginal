namespace LabServer.Shared.Models.Uni;

using System.Text.Json.Serialization;

using GitLab.Models.Group;

public class GroupCourseData : DataModel
{
    public System.Int64 Id { get; set; }
    public System.Int64 GroupId { get; set; }
    public System.Int64 CourseId { get; set; }
    public System.String CourseName { get; set; } = System.String.Empty;
    public System.Int64? GitLabGroupId { get; set; }
    [JsonIgnore]
    public System.Boolean GitLabSynced => GitLabGroupId != null;
    public GroupData? Group { get; set; }
    public CourseData? Course { get; set; }
    public GitLabGroup? GitLabGroup { get; set; }
    public IList<GroupCourseLabData>? GroupLabs { get; set; }

    public override void Update(DataModel other)
    {
        GroupCourseData update = other as GroupCourseData ?? throw new NotImplementedException();
        GitLabGroupId = update.GitLabGroupId;
        GitLabGroup = update.GitLabGroup;
    }
}