namespace LabServer.Server.Service;

using GitLab.Models;
using GitLab.Models.Group;
using GitLab.Models.MergeRequest;
using GitLab.Models.Project;
using GitLab.Models.User;

public interface IGitLab
{
    Task<ApiResult<T>> GetOne<T>(System.Int64 Id, System.String queryParams = "", System.String? endpointOverride = null) where T : GitLabModel;
    Task<ApiResult<GitLabUser>> CreateUser(CreateUserRequest createUserRequest);
    Task<ApiResult<GitLabProject>> CreateProject(CreateProjectRequest createProjectRequest);
    Task<ApiResult<GitLabGroup>> CreateGroup(CreateGroupRequest request);
    Task<List<GitLabProject>> GetProjects();
    Task<List<GitLabUser>> GetUsers();
    Task<List<GitLabMergeRequest>> GetMergeRequests();
}