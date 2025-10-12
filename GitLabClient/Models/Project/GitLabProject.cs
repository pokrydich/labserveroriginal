namespace GitLab.Models.Project;

using System.Text.Json.Serialization;

using GitLab.Models.User;

public enum PorjectAccessLevel
{
    NoAccess = 0,
    Guest = 10,
    Reporter = 20,
    Developper = 30,
    Maintainer = 40,
    Owner = 50
}

[GetEndpoint("/projects")]
public class GitLabProject : GitLabModel
{
    [JsonPropertyName("id")]
    public System.Int64 Id { get; set; }
    [JsonPropertyName("name")]
    public System.String Name { get; set; } = System.String.Empty;
    [JsonPropertyName("web_url")]
    public System.String WebUrl { get; set; } = System.String.Empty;

    public async Task<ApiResult<GitLabUser>> AddMember(GitLabUser user, PorjectAccessLevel accessLevel)
    {
        return await Client.Post<GitLabUser, AddProjectMemberRequest>(
            AddProjectMemberRequest.FromUser(user).WithAccess(accessLevel),
            $"/projects/{Id}/members"
        );
    }
}