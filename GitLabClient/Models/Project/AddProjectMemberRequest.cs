namespace GitLab.Models.Project;

using System.Text.Json.Serialization;

using GitLab.Models.User;

public class AddProjectMemberRequest
{
    [JsonPropertyName("user_id")]
    public System.Int64 UserId { get; set; }
    [JsonPropertyName("access_level")]
    public PorjectAccessLevel AccessLevel { get; set; } = PorjectAccessLevel.NoAccess;

    public AddProjectMemberRequest WithAccess(PorjectAccessLevel accessLevel)
    {
        AccessLevel = accessLevel;
        return this;
    }
    public static AddProjectMemberRequest FromUser(GitLabUser user) => new AddProjectMemberRequest
    {
        UserId = user.Id
    };
}