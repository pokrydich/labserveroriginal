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

    [Fact]
    public async Task Login_And_AssignTeacherRole_WorksCorrectly()
    {
        var login_admin_user = new
        {
            email = "test_mail_first@test.dev",
            password = "1qaz@WSX"
        };

        using var loginResponse = await _client.PostAsJsonAsync("api/Login", login_admin_user);
        Assert.True(loginResponse.IsSuccessStatusCode, $"Unexpected status {(int)loginResponse.StatusCode}");

        var loginJson = await loginResponse.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(loginJson);
        Assert.True(loginJson!["successful"]?.GetValue<bool>() ?? false, "Login must be successful");

        var token = loginJson["result"]?.ToString();
        Assert.False(string.IsNullOrWhiteSpace(token), "Token should not be null or empty");

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var url = "api/rest/Users/1/roles?role=1";
        using var response = await _client.GetAsync(url);

        Assert.True(response.IsSuccessStatusCode, $"Unexpected status {(int)response.StatusCode}");

        var json = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(json);

        Assert.True(json!["successful"]?.GetValue<bool>() ?? false, "Role assignment should be successful");
        Assert.Null(json["error"]);
        Assert.NotNull(json["warnings"]);
    }


    [Fact]
    public async Task Login_CreateGroup_And_Sync_WorksSuccessfully()
    {

        var login_teacher = new
        {
            email = "test_mail_second@test.dev",
            password = "1qaz@WSX"
        };

        using var loginResponse = await _client.PostAsJsonAsync("api/Login", login_teacher);
        Assert.True(loginResponse.IsSuccessStatusCode, $"Unexpected status {(int)loginResponse.StatusCode}");

        var loginJson = await loginResponse.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(loginJson);
        Assert.True(loginJson!["successful"]?.GetValue<bool>() ?? false, "Login must be successful");

        var token = loginJson["result"]?.ToString();
        Assert.False(string.IsNullOrWhiteSpace(token), "Token should not be null or empty");

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createGroupBody = new { name = "333" };

        using var createGroupResponse = await _client.PostAsJsonAsync("api/rest/Groups", createGroupBody);
        Assert.True(createGroupResponse.IsSuccessStatusCode, $"Unexpected status {(int)createGroupResponse.StatusCode}");

        var createGroupJson = await createGroupResponse.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(createGroupJson);
        Assert.True(createGroupJson!["successful"]?.GetValue<bool>() ?? false, "Group creation should be successful");
        Assert.Null(createGroupJson["error"]);

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        using var syncResponse = await _client.GetAsync("api/rest/Groups/sync", cts.Token);
        Assert.True(syncResponse.IsSuccessStatusCode, $"Unexpected status {(int)syncResponse.StatusCode}");

        var syncJson = await syncResponse.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(syncJson);
        Assert.True(syncJson!["successful"]?.GetValue<bool>() ?? false, "Group sync should be successful");
        Assert.Null(syncJson["error"]);
    }

}
