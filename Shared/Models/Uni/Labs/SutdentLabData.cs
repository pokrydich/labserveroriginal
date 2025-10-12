namespace LabServer.Shared.Models.Uni;

using System.Text.Json.Serialization;

using GitLab.Models.Project;

public enum StudentLabStatus
{
    cInProgress = 0,
    cOverdue = 1,
    cCompleted = 2
}

public class StudentLabData : DataModel
{
    public System.Int64 Id { get; set; }
    public System.Int64 StudentId { get; set; }
    public StudentData? Student { get; set; }
    public System.Int64 GroupCourseLabId { get; set; }
    public GroupCourseLabData? GroupCourseLab { get; set; }
    public DateTime OpenedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public StudentLabStatus Status { get; set; }
    public System.String Notes { get; set; } = System.String.Empty;
    public IList<StudentLabSubmissionData>? LabSubmissions { get; set; }
    public System.Int64? GitLabProjectId { get; set; }
    [JsonIgnore]
    public System.Boolean GitLabSynced => GitLabProjectId != null;
    public GitLabProject? GitLabProject { get; set; }

    public override void Update(DataModel other)
    {
        StudentLabData update = other as StudentLabData ?? throw new NotImplementedException();
        CompletedDate = update.CompletedDate;
        Status = update.Status;
        Notes = update.Notes;
        GitLabProjectId = update.GitLabProjectId;
        GitLabProject = update.GitLabProject;
    }
}