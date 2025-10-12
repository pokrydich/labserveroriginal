namespace GitLab.Models.Group;

using System.Text.Json.Serialization;

public class CreateGroupRequest
{
    [JsonPropertyName("name")]
    public System.String Name { get; set; } = System.String.Empty;
    [JsonPropertyName("path")]
    public System.String Path { get; set; } = System.String.Empty;
    [JsonPropertyName("parent_id")]
    public System.Int64? ParentId { get; set; }
}