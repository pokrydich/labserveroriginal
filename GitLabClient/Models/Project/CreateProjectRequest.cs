namespace GitLab.Models.Project;

using System.Text.Json.Serialization;

public class CreateProjectRequest
{
    [JsonPropertyName("name")]
    public System.String Name { get; set; } = System.String.Empty;
    [JsonPropertyName("namespace_id")]
    public System.Int64? NamespaceId { get; set; }
    [JsonPropertyName("initialize_with_readme")]
    public System.Boolean? InitializeWithReadme { get; set; }
    [JsonPropertyName("default_branch")]
    public System.String? DefaultBranch { get; set; }
}