namespace GitLab.Models.Note;

using System.Text.Json.Serialization;

public class GitLabNote : GitLabModel
{
    [JsonPropertyName("id")]
    public System.Int64 Id { get; set; }
    [JsonPropertyName("body")]
    public System.String Body { get; set; } = System.String.Empty;
}