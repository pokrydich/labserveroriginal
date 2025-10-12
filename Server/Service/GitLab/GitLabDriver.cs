namespace LabServer.Server.Service;

using Microsoft.Extensions.Configuration;

using GitLab;
using GitLab.Models.User;
using GitLab.Models.Project;
using System.Collections.Generic;
using GitLab.Models.Group;
using GitLab.Models;
using GitLab.Models.MergeRequest;

public class GitLabDriver : IGitLab
{
    private readonly GitLabClient _client;
    private readonly IConfiguration _configuration;
    private const System.String cGitLabConfigSection = "GitLabClient";
    public GitLabDriver(IConfiguration configuration)
    {
        _configuration = configuration;
        var gitlabConfiguration = _configuration.GetSection(cGitLabConfigSection);
        _client = new GitLabClient(
            gitlabConfiguration.GetValue<System.String>("url"),
            gitlabConfiguration.GetValue<System.String>("secret_token"));
    }

    public async Task<ApiResult<T>> GetOne<T>(System.Int64 Id, System.String queryParams = "", System.String? endpointOverride = null) where T : GitLabModel => await _client.GetOne<T>(Id, queryParams: queryParams, endpointOverride: endpointOverride);
    public async Task<ApiResult<GitLabUser>> CreateUser(CreateUserRequest request)
        => await _client.Post<GitLabUser, CreateUserRequest>(request);
    public async Task<ApiResult<GitLabProject>> CreateProject(CreateProjectRequest request)
        => await _client.Post<GitLabProject, CreateProjectRequest>(request);
    public async Task<ApiResult<GitLabGroup>> CreateGroup(CreateGroupRequest request)
        => await _client.Post<GitLabGroup, CreateGroupRequest>(request);
    public async Task<List<GitLabProject>> GetProjects() => await _client.Get<GitLabProject>();
    public async Task<List<GitLabUser>> GetUsers() => await _client.Get<GitLabUser>();
    public async Task<List<GitLabGroup>> GetGroups() => await _client.Get<GitLabGroup>();
    public async Task<List<GitLabMergeRequest>> GetMergeRequests() => await _client.Get<GitLabMergeRequest>(queryParams: "scope=assigned_to_me");
}