namespace Server.IntegrationTests.GitLab;

using System.Net;
using Server.IntegrationTests.Infrastructure;

public class GitLabAvailabilityTests : IClassFixture<HttpClientFixture>
{
    private readonly HttpClient _client;

    public GitLabAvailabilityTests(HttpClientFixture fixture)
    {
        _client = fixture.CreateClient("LABSERVER_GITLAB_BASE", "http://localhost:8080/");
    }

    [Fact]
    public async Task RootEndpoint_Responds()
    {
        for (var attempt = 0; attempt < 10; attempt++)
        {
            using var response = await _client.GetAsync(string.Empty);
            if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.Found || response.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                Assert.True(true);
                return;
            }

            await Task.Delay(TimeSpan.FromSeconds(30));
        }

        throw new InvalidOperationException("GitLab did not respond with an expected status within the allotted time.");
    }
}
