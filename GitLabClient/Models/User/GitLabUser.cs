namespace GitLab.Models.User;

using System.Text.Json.Serialization;

[GetEndpoint("/users")]
public class GitLabUser : GitLabModel
{
    [JsonPropertyName("id")]
    public System.Int64 Id { get; set; }
    [JsonPropertyName("username")]
    public System.String Username { get; set; } = System.String.Empty;
    [JsonPropertyName("name")]
    public System.String Name { get; set; } = System.String.Empty;
    [JsonPropertyName("email")]
    public System.String Email { get; set; } = System.String.Empty;
    [JsonPropertyName("external")]
    public System.Boolean IsExternal { get; set; }
    [JsonPropertyName("web_url")]
    public System.String WebUrl { get; set; } = System.String.Empty;
}