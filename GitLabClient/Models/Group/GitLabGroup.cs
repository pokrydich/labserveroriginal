namespace GitLab.Models.Group;

using System.Text.Json.Serialization;

[GetEndpoint("/groups")]
public class GitLabGroup : GitLabModel
{
    [JsonPropertyName("id")]
    public System.Int64 Id { get; set; }
    [JsonPropertyName("name")]
    public System.String Name { get; set; } = System.String.Empty;
    [JsonPropertyName("path")]
    public System.String Path { get; set; } = System.String.Empty;
    [JsonPropertyName("parent_id")]
    public System.Int64? ParentId { get; set; }
    [JsonPropertyName("web_url")]
    public System.String WebUrl { get; set; } = System.String.Empty;

    public async Task<ApiResult<GitLabGroup>> CreateSubgroup(CreateGroupRequest request)
    {
        request.ParentId = Id;
        return await Client.Post<GitLabGroup, CreateGroupRequest>(request);
    }
}