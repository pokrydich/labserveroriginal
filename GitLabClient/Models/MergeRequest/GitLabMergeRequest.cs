namespace GitLab.Models.MergeRequest;

using System.Globalization;
using System.Text.Json.Serialization;

using GitLab.Models.Note;

public enum MergeRequestState
{
    Opened = 0,
    Closed = 1,
    Locked = 2,
    Merged = 3
}

public class MergeUserInfo
{
    [JsonPropertyName("id")]
    public System.Int64 Id { get; set; }
    [JsonPropertyName("username")]
    public System.String Username { get; set; } = System.String.Empty;
    [JsonPropertyName("name")]
    public System.String Name { get; set; } = System.String.Empty;
}

[GetEndpoint("/merge_requests")]
public class GitLabMergeRequest : GitLabModel
{
    [JsonPropertyName("id")]
    public System.Int64 Id { get; set; }
    [JsonPropertyName("iid")]
    public System.Int64 Iid { get; set; }
    [JsonPropertyName("state")]
    public System.String StateRaw { get; set; } = System.String.Empty;
    [JsonIgnore]
    public MergeRequestState State => Enum.Parse<MergeRequestState>(StateRaw, true);
    [JsonPropertyName("title")]
    public System.String Title { get; set; } = System.String.Empty;
    [JsonPropertyName("project_id")]
    public System.Int64 ProjectId { get; set; }
    [JsonPropertyName("created_at")]
    public System.String CreatedAtRaw { get; set; } = System.String.Empty;
    [JsonIgnore]
    public DateTime CreatedAt => DateTime.ParseExact(CreatedAtRaw, "yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture);
    [JsonPropertyName("author")]
    public MergeUserInfo Author { get; set; }
    [JsonPropertyName("assignees")]
    public List<MergeUserInfo> Assignees { get; set; }
    [JsonPropertyName("source_project_id")]
    public System.Int64 SourceProjectId { get; set; }
    [JsonPropertyName("target_project_id")]
    public System.Int64 TargetProjectId { get; set; }
    [JsonPropertyName("sha")]
    public System.String CommitHash { get; set; } = System.String.Empty;
    [JsonPropertyName("web_url")]
    public System.String WebUrl { get; set; } = System.String.Empty;


    public async Task<ApiResult<GitLabNote>> AddNote(System.String body)
    {
        return await Client.Post<GitLabNote, CreateNoteRequest>(
            new CreateNoteRequest { Body = body },
            $"/projects/{ProjectId}/merge_requests/{Iid}/notes"
        );
    }
}