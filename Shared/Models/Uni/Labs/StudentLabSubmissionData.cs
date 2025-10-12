namespace LabServer.Shared.Models.Uni;

using System.Text.Json.Serialization;

using GitLab.Models.MergeRequest;

public enum StudentLabSubmissionStatus
{
    cActive = 0, // current lates lab submission (test will be executed for such submissions)
    cRevoked, // user revoked merge request
    cDeprecated // newer submission is available
}

public class StudentLabSubmissionData : DataModel
{
    public System.Int64 Id { get; set; }
    public System.Int64 StudentLabId { get; set; }
    public StudentLabSubmissionStatus Status { get; set; }
    public DateTime SubmittedDate { get; set; }
    public StudentLabData? StudentLab { get; set; }
    public System.Int64? GitLabMergeRequestId { get; set; }
    public System.Int64? GitLabMergeRequestIid { get; set; }
    [JsonIgnore]
    public System.Boolean GitLabSynced => GitLabMergeRequestId != null;
    public GitLabMergeRequest? GitLabMergeRequest { get; set; }

    public override void Update(DataModel other)
    {
        StudentLabSubmissionData update = other as StudentLabSubmissionData ?? throw new NotImplementedException();
        Status = update.Status;
        SubmittedDate = update.SubmittedDate;
    }
}