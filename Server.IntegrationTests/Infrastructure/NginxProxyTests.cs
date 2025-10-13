namespace Server.IntegrationTests.Infrastructure;

public class NginxProxyTests : IClassFixture<HttpClientFixture>
{
    private readonly HttpClient _client;

    public NginxProxyTests(HttpClientFixture fixture)
    {
        _client = fixture.CreateClient("LABSERVER_NGINX_BASE", "http://localhost/");
    }

    [Fact]
    public async Task Proxy_Forwards_Login_Request()
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/Login")
        {
            Content = JsonContent.Create(new { Email = "integration@test.dev", Password = "invalid" })
        };

        using var response = await _client.SendAsync(request);

        Assert.True(response.IsSuccessStatusCode, $"Nginx proxy failed with {(int)response.StatusCode}");
    }
}
