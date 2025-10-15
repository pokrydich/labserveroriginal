namespace Server.IntegrationTests.Api;

using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;
using Server.IntegrationTests.Infrastructure;

public class LabServerApiTests : IClassFixture<HttpClientFixture>
{
    private readonly HttpClient _client;

    public LabServerApiTests(HttpClientFixture fixture)
    {
        _client = fixture.CreateClient("LABSERVER_API_BASE", "http://localhost/");
        Console.WriteLine($"[DEBUG] HttpClient created. BaseAddress = {_client.BaseAddress}");
    }

    private static async Task LogResponse(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"[DEBUG] → HTTP {(int)response.StatusCode} {response.ReasonPhrase}");
        Console.WriteLine($"[DEBUG] → Response body: {content}");
    }

    [Fact]
    public async Task Register_SameUsers_OneTwice_ReturnsAlreadyTakenError()
    {
        Console.WriteLine("[DEBUG] Starting test: Register_SameUsers_OneTwice_ReturnsAlreadyTakenError");

        var user1 = new { email = "test_mail_first@test.dev", password = "1qaz@WSX", confirmPassword = "1qaz@WSX" };
        var user2 = new { email = "test_mail_second@test.dev", password = "1qaz@WSX", confirmPassword = "1qaz@WSX" };

        Console.WriteLine("[DEBUG] Registering first user...");
        using (var firstResponse = await _client.PostAsJsonAsync("api/Accounts", user1))
        {
            await LogResponse(firstResponse);
            Assert.True(firstResponse.IsSuccessStatusCode);
        }

        Console.WriteLine("[DEBUG] Registering second user...");
        using (var secondResponse = await _client.PostAsJsonAsync("api/Accounts", user2))
        {
            await LogResponse(secondResponse);
            Assert.True(secondResponse.IsSuccessStatusCode);
        }

        Console.WriteLine("[DEBUG] Trying to register first user again (should fail)...");
        using (var duplicateResponse = await _client.PostAsJsonAsync("api/Accounts", user1))
        {
            await LogResponse(duplicateResponse);
            Assert.True(duplicateResponse.IsSuccessStatusCode);

            var json = await duplicateResponse.Content.ReadFromJsonAsync<JsonObject>();
            Assert.NotNull(json);
            Assert.False(json!["successful"]?.GetValue<bool>() ?? true);

            var error = json["error"]?.ToString() ?? string.Empty;
            Console.WriteLine($"[DEBUG] Error message: {error}");
            Assert.Contains("already taken", error, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public async Task Login_ReturnsErrorEnvelope()
    {
        Console.WriteLine("[DEBUG] Starting test: Login_ReturnsErrorEnvelope");

        var login_user = new { email = "test_mail@test.dev", password = "invalid" };

        using var response = await _client.PostAsJsonAsync("api/Login", login_user);
        await LogResponse(response);

        Assert.True(response.IsSuccessStatusCode);
        var json = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(json);
        Assert.False(json!["Successful"]?.GetValue<bool>() ?? true);
    }

    [Fact]
    public async Task Login_ReturnsSuccessEnvelope()
    {
        Console.WriteLine("[DEBUG] Starting test: Login_ReturnsSuccessEnvelope");

        var login_user = new { email = "test_mail_first@test.dev", password = "1qaz@WSX" };

        using var response = await _client.PostAsJsonAsync("api/Login", login_user);
        await LogResponse(response);

        Assert.True(response.IsSuccessStatusCode);
        var json = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(json);

        Assert.True(json!["successful"]?.GetValue<bool>() ?? false);
        Assert.Null(json["error"]);
        Assert.NotNull(json["warnings"]);

        var token = json["result"]?.ToString();
        Console.WriteLine($"[DEBUG] Received token: {token}");
        Assert.False(string.IsNullOrWhiteSpace(token));
    }

    [Fact]
    public async Task Login_And_AssignTeacherRole_WorksCorrectly()
    {
        Console.WriteLine("[DEBUG] Starting test: Login_And_AssignTeacherRole_WorksCorrectly");

        var login_admin_user = new { email = "test_mail_first@test.dev", password = "1qaz@WSX" };

        using var loginResponse = await _client.PostAsJsonAsync("api/Login", login_admin_user);
        await LogResponse(loginResponse);

        Assert.True(loginResponse.IsSuccessStatusCode);
        var loginJson = await loginResponse.Content.ReadFromJsonAsync<JsonObject>();
        var token = loginJson?["result"]?.ToString();
        Console.WriteLine($"[DEBUG] Login successful, token = {token}");
        Assert.False(string.IsNullOrWhiteSpace(token));

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        Console.WriteLine("[DEBUG] Authorization header set.");

        var url = "api/rest/Users/1/roles?role=1";
        Console.WriteLine($"[DEBUG] Sending GET {url}");
        using var response = await _client.GetAsync(url);
        await LogResponse(response);

        Assert.True(response.IsSuccessStatusCode);
        var json = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.True(json!["successful"]?.GetValue<bool>() ?? false);
    }

    [Fact]
    public async Task Login_CreateGroup_And_Sync_WorksSuccessfully()
    {
        Console.WriteLine("[DEBUG] Starting test: Login_CreateGroup_And_Sync_WorksSuccessfully");

        var login_teacher = new { email = "test_mail_second@test.dev", password = "1qaz@WSX" };

        using var loginResponse = await _client.PostAsJsonAsync("api/Login", login_teacher);
        await LogResponse(loginResponse);

        Assert.True(loginResponse.IsSuccessStatusCode);
        var loginJson = await loginResponse.Content.ReadFromJsonAsync<JsonObject>();
        var token = loginJson?["result"]?.ToString();
        Console.WriteLine($"[DEBUG] Login successful, token = {token}");
        Assert.False(string.IsNullOrWhiteSpace(token));

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        Console.WriteLine("[DEBUG] Authorization header set.");

        var createGroupBody = new { name = "333" };
        Console.WriteLine("[DEBUG] Sending POST api/rest/Groups");
        using var createGroupResponse = await _client.PostAsJsonAsync("api/rest/Groups", createGroupBody);
        await LogResponse(createGroupResponse);

        Assert.True(createGroupResponse.IsSuccessStatusCode);
        var createGroupJson = await createGroupResponse.Content.ReadFromJsonAsync<JsonObject>();
        Assert.True(createGroupJson!["successful"]?.GetValue<bool>() ?? false);

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        Console.WriteLine("[DEBUG] Sending GET api/rest/Groups/sync");
        using var syncResponse = await _client.GetAsync("api/rest/Groups/sync", cts.Token);
        await LogResponse(syncResponse);

        Assert.True(syncResponse.IsSuccessStatusCode);
        var syncJson = await syncResponse.Content.ReadFromJsonAsync<JsonObject>();
        Assert.True(syncJson!["successful"]?.GetValue<bool>() ?? false);
    }
}
