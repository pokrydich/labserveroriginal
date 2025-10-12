namespace LabServer.Shared.Models.TestAPI;

using System.Globalization;
using System.Text.Json.Serialization;

using GitLab.Models.MergeRequest;

public class MergeRequestData
{
    [JsonPropertyName("title")]
    public System.String Title { get; set; } = System.String.Empty;
    [JsonPropertyName("source_project_id")]
    public System.Int64 SourceProjectId { get; set; }
    [JsonPropertyName("sha")]
    public System.String CommitHash { get; set; } = System.String.Empty;
}

public class ScheduleTestRequestModel
{
    public MergeRequestData MergeRequest { get; set; }
    public System.String RepoArchiveBase64 { get; set; }
    [JsonIgnore]
    public System.Byte[] RepoArchive => Convert.FromBase64String(RepoArchiveBase64);
}