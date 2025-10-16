namespace Server.IntegrationTests.Api;

using System.Net.Http.Headers;
using System.Text;
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

    private async Task<string> LoginAndGetToken(string email, string password)
    {
        var body = new { email, password };
        using var response = await _client.PostAsJsonAsync("api/Login", body);
        await LogResponse(response);

        Assert.True(response.IsSuccessStatusCode, $"Login failed for {email}");
        var json = await response.Content.ReadFromJsonAsync<JsonObject>();
        var token = json?["result"]?.ToString();
        Assert.False(string.IsNullOrWhiteSpace(token), $"Token is empty for {email}");

        Console.WriteLine($"[DEBUG] Logged in as {email}, token acquired.");
        return token!;
    }


    [Fact]
    public async Task Register_SameUsers_OneTwice_ReturnsAlreadyTakenError()
    {
        Console.WriteLine("[DEBUG] Starting test: Register_SameUsers_OneTwice_ReturnsAlreadyTakenError");

        var user1 = new { email = "test_mail_first@test.dev", password = "1qaz@WSX", confirmPassword = "1qaz@WSX" };
        var user2 = new { email = "test_mail_second@test.dev", password = "1qaz@WSX", confirmPassword = "1qaz@WSX" };
        var user3 = new { email = "test_mail_third@test.dev", password = "1qaz@WSX", confirmPassword = "1qaz@WSX" };

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

        Console.WriteLine("[DEBUG] Registering second user...");
        using (var thirdResponse = await _client.PostAsJsonAsync("api/Accounts", user3))
        {
            await LogResponse(thirdResponse);
            Assert.True(thirdResponse.IsSuccessStatusCode);
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
    public async Task Login_And_AssignTeacherAndAssistentRole_WorksCorrectly()
    {
        Console.WriteLine("[DEBUG] Starting test: Login_And_AssignTeacherAndAssistentRole_WorksCorrectly");

        var token = await LoginAndGetToken("test_mail_first@test.dev", "1qaz@WSX");
        Console.WriteLine($"[DEBUG] Login successful, token = {token}");
        Assert.False(string.IsNullOrWhiteSpace(token));

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        Console.WriteLine("[DEBUG] Authorization header set.");

        var userId = 2; // 1 - administrator
        var roleId = 1; // {0:"administrator", 1:"teacher", 2:"assistent"}

        await AddUserRoleAsync(userId, roleId);

        userId = 3;
        roleId = 2;

        await AddUserRoleAsync(userId, roleId);

    }

    private async Task AddUserRoleAsync(int userId, int roleId)
    {
        var url = $"api/rest/Users/{userId}/roles?role={roleId}";
        Console.WriteLine($"[DEBUG] Sending GET {url}");

        using var response = await _client.GetAsync(url);
        await LogResponse(response);

        Assert.True(response.IsSuccessStatusCode, $"[ERROR] Request to {url} failed with status {response.StatusCode}");
        
        var addRoleJson = await response.Content.ReadFromJsonAsync<JsonObject>();
        var successful = addRoleJson!["successful"]?.GetValue<bool>() ?? false;
        var error = addRoleJson["error"]?.ToString();
        Assert.True(successful, $"Failed to add role '{roleId}' with user id '{userId}'. successful=false. Error: {error}");
    }

    [Fact]
    public async Task Login_CreateGroup_And_Sync_WorksSuccessfully()
    {
        Console.WriteLine("[DEBUG] Starting test: Login_CreateGroup_And_Sync_WorksSuccessfully");

        var token = await LoginAndGetToken("test_mail_second@test.dev", "1qaz@WSX");
        Console.WriteLine($"[DEBUG] Login successful, token = {token}");

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        Console.WriteLine("[DEBUG] Authorization header set.");


        var createGroupBody = new { name = "333" };
        Console.WriteLine("[DEBUG] Sending POST api/rest/Groups");
        using var createGroupResponse = await _client.PostAsJsonAsync("api/rest/Groups", createGroupBody);
        await LogResponse(createGroupResponse);

        Assert.True(createGroupResponse.IsSuccessStatusCode);
        var createGroupJson = await createGroupResponse.Content.ReadFromJsonAsync<JsonObject>();
        var successful = createGroupJson!["successful"]?.GetValue<bool>() ?? false;
        var error = createGroupJson["error"]?.ToString();
        Assert.True(successful, $"Falid to create group with name '333'. successful=false. Error: {error}");

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
        Console.WriteLine("[DEBUG] Sending GET api/rest/Groups/sync");
        using var syncResponse = await _client.GetAsync("api/rest/Groups/sync", cts.Token);
        await LogResponse(syncResponse);

        Assert.True(syncResponse.IsSuccessStatusCode);
        var syncJson = await syncResponse.Content.ReadFromJsonAsync<JsonObject>();
        successful = syncJson!["successful"]?.GetValue<bool>() ?? false;
        error = syncJson["error"]?.ToString();
        Assert.True(successful, $"Unsuccessful synchronization of groups with GitLab. successful=false. Error: {error}");
    }

    [Fact]
    public async Task Login_And_ImportStudents_FromCsv_WorksSuccessfully()
    {
        Console.WriteLine("[DEBUG] Starting test: Login_And_ImportStudents_FromCsv_WorksSuccessfully");

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

        var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var csvPath = Path.Combine(projectRoot, "CI-CD", "students.csv");
        Console.WriteLine($"[DEBUG] CSV path = {csvPath}");
        var textFromCsv = BuildTextForImportFromCsv(csvPath);
        Console.WriteLine($"[DEBUG] text length = {textFromCsv.Length}");

        var groupId = 1;
        var body = new { text = textFromCsv };

        Console.WriteLine($"[DEBUG] Sending POST api/rest/Groups/{groupId}/students/import");
        using var importResponse = await _client.PostAsJsonAsync($"api/rest/Groups/{groupId}/students/import", body);
        await LogResponse(importResponse);

        Assert.True(importResponse.IsSuccessStatusCode);
        var importJson = await importResponse.Content.ReadFromJsonAsync<JsonObject>();
        var successful = importJson!["successful"]?.GetValue<bool>() ?? false;
        var error = importJson["error"]?.ToString();
        Assert.True(successful, $"Failed to import cvs database. successful=false. Error: {error}");
        

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
        Console.WriteLine($"[DEBUG] Sending GET /api/rest/Groups/{groupId}/students/sync");
        using var syncResponse = await _client.GetAsync($"/api/rest/Groups/{groupId}/students/sync", cts.Token);
        await LogResponse(syncResponse);

        Assert.True(syncResponse.IsSuccessStatusCode);
        var syncJson = await syncResponse.Content.ReadFromJsonAsync<JsonObject>();
        successful = syncJson!["successful"]?.GetValue<bool>() ?? false;
        error = syncJson["error"]?.ToString();
        Assert.True(successful, $"Unsuccessful synchronization of students with GitLab in group '{groupId}'. successful=false. Error: {error}");
    }


    private static string BuildTextForImportFromCsv(string csvPath)
    {
        if (!File.Exists(csvPath))
            throw new FileNotFoundException("CSV file not found", csvPath);

        var lines = File.ReadAllLines(csvPath);
        if (lines.Length == 0) return string.Empty;

        var sb = new StringBuilder(capacity: Math.Max(128, lines.Length * 32));

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = SplitTwoColumns(line);
            if (parts == null) continue;

            var name = TrimQuotes(parts.Value.name);
            var email = TrimQuotes(parts.Value.email);

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
                continue;

            sb.Append(name).Append(',').Append(email).Append('\n');
        }

        return sb.ToString();
    }

    private static (string name, string email)? SplitTwoColumns(string line)
    {
        var parts = line.Split(',', 2);
        if (parts.Length < 2) return null;
        return (parts[0].Trim(), parts[1].Trim());
    }

    private static string TrimQuotes(string s)
    {
        if (s.Length >= 2 && s[0] == '"' && s[^1] == '"')
            return s[1..^1].Trim();
        return s.Trim();
    }

    [Fact]
    public async Task Login_And_CreateCourse_WorksSuccessfully()
    {
        Console.WriteLine("[DEBUG] Starting test: Login_And_CreateCourse_WorksSuccessfully");

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

        var createCourseBody = new { name = "test_course" };
        Console.WriteLine("[DEBUG] Sending POST api/rest/Courses");
        using var createCourseResponse = await _client.PostAsJsonAsync("api/rest/Courses", createCourseBody);
        await LogResponse(createCourseResponse);

        var createCourseJson = await createCourseResponse.Content.ReadFromJsonAsync<JsonObject>();
        var successful = createCourseJson!["successful"]?.GetValue<bool>() ?? false;
        var error = createCourseJson["error"]?.ToString();
        Assert.True(successful, $"Course creation failed. successful=false. Error: {error}");

        Console.WriteLine("[DEBUG] Course created successfully!");
    }


    [Fact]
    public async Task Login_And_CreateTwoLabs_WorksSuccessfully()
    {
        Console.WriteLine("[DEBUG] Starting test: Login_And_CreateTwoLabs_WorksSuccessfully");

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

        var courseId = 7;

        await CreateLab(courseId, "test_lab_1");
        await CreateLab(courseId, "test_lab_2");

        Console.WriteLine("[DEBUG] Both labs created successfully!");


        using var labsResponse = await _client.GetAsync($"api/rest/Courses/{courseId}/labs");
        await LogResponse(labsResponse);

        Assert.True(labsResponse.IsSuccessStatusCode, "Labs GET request failed");

        var labsJson = await labsResponse.Content.ReadFromJsonAsync<JsonObject>();
        Assert.True(labsJson!["successful"]?.GetValue<bool>() ?? false, "Labs response was not successful");

        var labsArray = labsJson["result"]?.AsArray();
        Assert.NotNull(labsArray);

        var labNames = labsArray!
            .Select(l => l?["name"]?.ToString())
            .Where(n => !string.IsNullOrEmpty(n))
            .ToList();

        Console.WriteLine($"[DEBUG] Found labs: {string.Join(", ", labNames)}");

        Assert.Contains("test_lab_1", labNames);
        Assert.Contains("test_lab_2", labNames);

        Console.WriteLine("[DEBUG] Labs test_lab_1 and test_lab_2 found successfully!");
    }

    private async Task CreateLab(int courseId, string labName)
    {
        var createLabBody = new { name = labName };

        Console.WriteLine($"[DEBUG] Sending POST api/rest/Courses/{courseId}/labs with name = {labName}");
        using var createLabResponse = await _client.PostAsJsonAsync($"api/rest/Courses/{courseId}/labs", createLabBody);
        await LogResponse(createLabResponse);

        var createLabJson = await createLabResponse.Content.ReadFromJsonAsync<JsonObject>();
        var successful = createLabJson!["successful"]?.GetValue<bool>() ?? false;
        var error = createLabJson["error"]?.ToString();
        Assert.True(successful, $"Failed to create lab '{labName}'. successful=false. Error: {error}");
    }

    [Fact]
    public async Task AccessChanges_WhenRoleIsRemoved()
    {
        Console.WriteLine("[DEBUG] Starting test: AccessChanges_WhenRoleIsRemoved");

        var tokenThirdUser = await LoginAndGetToken("test_mail_third@test.dev", "1qaz@WSX");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenThirdUser);

        Console.WriteLine("[DEBUG] Getting groups as test_mail_third...");
        using var firstGroupsResponse = await _client.GetAsync("api/rest/Groups");
        await LogResponse(firstGroupsResponse);
        Assert.True(firstGroupsResponse.IsSuccessStatusCode, "First groups GET failed");

        var createCourseJson = await firstGroupsResponse.Content.ReadFromJsonAsync<JsonObject>();
        var successful = createCourseJson!["successful"]?.GetValue<bool>() ?? false;
        Assert.True(successful, $"User doesn't have access to GET groups (not enough privileges). successful=false.");


        var tokenFirst = await LoginAndGetToken("test_mail_first@test.dev", "1qaz@WSX");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenFirst);

        var userId = 3;
        var roleId = 2;

        Console.WriteLine("[DEBUG] Removing assistent role from user...");
        using var deleteRoleResponse = await _client.DeleteAsync($"api/rest/Users/{userId}/roles?role={roleId}");
        await LogResponse(deleteRoleResponse);
        Assert.True(deleteRoleResponse.IsSuccessStatusCode, "Role removal failed");


        tokenThirdUser = await LoginAndGetToken("test_mail_third@test.dev", "1qaz@WSX");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenThirdUser);

        Console.WriteLine("[DEBUG] Getting groups again as test_mail_third...");
        using var secondGroupsResponse = await _client.GetAsync("api/rest/Groups");
        await LogResponse(secondGroupsResponse);

        Assert.True(secondGroupsResponse.StatusCode == System.Net.HttpStatusCode.Forbidden, "User access without appropriate privileges");

    }

}