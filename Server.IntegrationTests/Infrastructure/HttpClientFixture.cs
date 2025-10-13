namespace Server.IntegrationTests.Infrastructure;

using System.Net.Http.Headers;

public sealed class HttpClientFixture : IDisposable
{
    private readonly List<HttpClient> _clients = new();

    public HttpClient CreateClient(string variableName, string defaultBaseAddress)
    {
        var baseAddress = Environment.GetEnvironmentVariable(variableName) ?? defaultBaseAddress;
        var client = new HttpClient
        {
            BaseAddress = new Uri(baseAddress, UriKind.Absolute)
        };
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _clients.Add(client);
        return client;
    }

    public void Dispose()
    {
        foreach (var client in _clients)
        {
            client.Dispose();
        }
    }
}
