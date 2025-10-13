namespace Server.IntegrationTests.Api;

using System.Text.Json;
using System.Text.Json.Nodes;
using Server.IntegrationTests.Infrastructure;

public class LabServerApiTests : IClassFixture<HttpClientFixture>
{
    private readonly HttpClient _client;

    public LabServerApiTests(HttpClientFixture fixture)
    {
        _client = fixture.CreateClient("LABSERVER_API_BASE", "http://localhost/");
    }

    [Fact]
    public async Task Login_ReturnsErrorEnvelope()
    {
        using var response = await _client.PostAsJsonAsync("api/Login", new { Email = "integration@test.dev", Password = "invalid" });

        Assert.True(response.IsSuccessStatusCode, $"Unexpected status {(int)response.StatusCode}");

        var json = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(json);
        Assert.False(json!["Successful"]?.GetValue<bool>() ?? true);
    }

    [Fact]
    public async Task Students_Endpoint_DoesNotFail()
    {
        using var response = await _client.GetAsync("api/students");

        Assert.DoesNotContain((int)response.StatusCode, new[] { 500, 502, 503, 504 });
    }

    [Fact]
    public async Task GitLabSync_AllowsExpectedStatuses()
    {
        using var response = await _client.PostAsJsonAsync("api/gitlab/sync", new { groupId = 1 });

        var acceptable = new[] { 200, 202, 400, 401, 403, 404 };
        Assert.Contains((int)response.StatusCode, acceptable);
    }
}
