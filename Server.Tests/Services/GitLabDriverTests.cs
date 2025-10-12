namespace LabServer.Server.Tests.Services;

using System.Net;
using System.Net.Http;
using GitLab.Models.MergeRequest;
using GitLab.Models.Project;
using LabServer.Server.Service;
using LabServer.Server.Tests.TestFixtures;

public class GitLabDriverTests
{
    private static GitLabDriver CreateDriver(Func<HttpRequestMessage, HttpResponseMessage> handler)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["GitLabClient:url"] = "http://gitlab.test",
                ["GitLabClient:secret_token"] = "token"
            })
            .Build();

        var driver = new GitLabDriver(configuration);

        var httpClient = new HttpClient(new StubHttpMessageHandler(handler));
        var gitLabClient = new GitLab.GitLabClient("http://gitlab.test", "token");

        var clientField = typeof(GitLab.GitLabClient).GetField("_httpClient", BindingFlags.Instance | BindingFlags.NonPublic);
        clientField!.SetValue(gitLabClient, httpClient);

        var driverField = typeof(GitLabDriver).GetField("_client", BindingFlags.Instance | BindingFlags.NonPublic);
        driverField!.SetValue(driver, gitLabClient);

        return driver;
    }

    [Fact]
    public async Task GetProjects_ReturnsPayload()
    {
        var driver = CreateDriver(request =>
        {
            Assert.Contains("/api/v4/projects", request.RequestUri!.AbsoluteUri);
            Assert.Contains("private_token=token", request.RequestUri!.Query);
            return StubHttpMessageHandler.JsonResponse(new[]
            {
                new GitLabProject
                {
                    Id = 1,
                    Name = "TestProject",
                    WebUrl = "http://gitlab.test/project"
                }
            });
        });

        var projects = await driver.GetProjects();

        Assert.Single(projects);
        Assert.Equal("TestProject", projects[0].Name);
    }

    [Fact]
    public async Task GetMergeRequests_UsesAssignedScope()
    {
        var driver = CreateDriver(request =>
        {
            Assert.Contains("/api/v4/merge_requests", request.RequestUri!.AbsoluteUri);
            Assert.Contains("scope=assigned_to_me", request.RequestUri!.Query);
            var payload = new[]
            {
                new GitLabMergeRequest
                {
                    Id = 10,
                    Iid = 5,
                    ProjectId = 1,
                    TargetProjectId = 2,
                    SourceProjectId = 3,
                    StateRaw = "opened",
                    Title = "MR",
                    CreatedAtRaw = "2025-01-01T00:00:00.000Z",
                    Author = new MergeUserInfo { Id = 99, Username = "user", Name = "User" },
                    Assignees = new List<MergeUserInfo>(),
                    CommitHash = "abc",
                    WebUrl = "http://gitlab.test/mr"
                }
            };
            return StubHttpMessageHandler.JsonResponse(payload);
        });

        var mergeRequests = await driver.GetMergeRequests();

        Assert.Single(mergeRequests);
        Assert.Equal(10, mergeRequests[0].Id);
    }
}
