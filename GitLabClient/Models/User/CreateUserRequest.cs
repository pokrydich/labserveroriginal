namespace GitLab.Models.User;

using System.Text.Json.Serialization;

public class CreateUserRequest
{
    [JsonPropertyName("username")] // TODO: why does it not work?!
    public System.String Username { get; set; } = System.String.Empty;
    [JsonPropertyName("name")]
    public System.String Name { get; set; } = System.String.Empty;
    [JsonPropertyName("email")]
    public System.String Email { get; set; } = System.String.Empty;
    [JsonPropertyName("password")]
    public System.String Password { get; set; } = System.String.Empty;
    [JsonPropertyName("external")]
    public System.Boolean IsExternal { get; set; } = false;
    [JsonPropertyName("skip_confirmation")]
    public System.Boolean SkipConfirmation { get; set; } = false;
}