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
    public async Task Register_SameUsers_OneTwice_ReturnsAlreadyTakenError()
    {
        var user1 = new
        {
            email = "test_mail_first@test.dev",
            password = "1qaz@WSX",
            confirmPassword = "1qaz@WSX"
        };

        var user2 = new
        {
            email = "test_mail_second@test.dev",
            password = "1qaz@WSX",
            confirmPassword = "1qaz@WSX"
        };

        using (var firstResponse = await _client.PostAsJsonAsync("api/Accounts", user1))
        {
            Assert.True(firstResponse.IsSuccessStatusCode, $"Unexpected status {(int)firstResponse.StatusCode}");

            var firstJson = await firstResponse.Content.ReadFromJsonAsync<JsonObject>();
            Assert.NotNull(firstJson);
            Assert.True(firstJson!["successful"]?.GetValue<bool>() ?? false);
            Assert.Null(firstJson["error"]);
        }

        using (var firstResponse = await _client.PostAsJsonAsync("api/Accounts", user2))
        {
            Assert.True(firstResponse.IsSuccessStatusCode, $"Unexpected status {(int)firstResponse.StatusCode}");

            var firstJson = await firstResponse.Content.ReadFromJsonAsync<JsonObject>();
            Assert.NotNull(firstJson);
            Assert.True(firstJson!["successful"]?.GetValue<bool>() ?? false);
            Assert.Null(firstJson["error"]);
        }

        using (var secondResponse = await _client.PostAsJsonAsync("api/Accounts", user1))
        {
            Assert.True(secondResponse.IsSuccessStatusCode, $"Unexpected status {(int)secondResponse.StatusCode}");

            var secondJson = await secondResponse.Content.ReadFromJsonAsync<JsonObject>();
            Assert.NotNull(secondJson);

            Assert.False(secondJson!["successful"]?.GetValue<bool>() ?? true);
            var errorMessage = secondJson["error"]?.ToString() ?? string.Empty;
            Assert.Contains("already taken", errorMessage, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public async Task Login_ReturnsErrorEnvelope()
    {
        var login_user = new
        {
            email = "test_mail@test.dev",
            password = "invalid"
        };

        using var response = await _client.PostAsJsonAsync("api/Login", login_user);

        Assert.True(response.IsSuccessStatusCode, $"Unexpected status {(int)response.StatusCode}");

        var json = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(json);
        Assert.False(json!["Successful"]?.GetValue<bool>() ?? true);
    }


    [Fact]
    public async Task Login_ReturnsSuccessEnvelope()
    {

        var login_user = new
        {
            email = "test_mail_first@test.dev",
            password = "1qaz@WSX"
        };

        using var response = await _client.PostAsJsonAsync("api/Login", login_user);

        Assert.True(response.IsSuccessStatusCode, $"Unexpected status {(int)response.StatusCode}");

        var json = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(json);

        Assert.True(json!["successful"]?.GetValue<bool>() ?? false);
        Assert.Null(json["error"]);

        Assert.NotNull(json["warnings"]);

        var token = json["result"]?.ToString();
        Assert.False(string.IsNullOrWhiteSpace(token), "Result (token) should not be empty");
    }

}
