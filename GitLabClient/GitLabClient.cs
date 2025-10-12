namespace GitLab;

using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using GitLab.Models;

public class GitLabClient
{
    private System.String _apiBase;
    private System.String _authToken;
    private HttpClient _httpClient;

    public GitLabClient(System.String gitlabUrl, System.String authToken)
    {
        _apiBase = $"{gitlabUrl}/api/v4";
        _authToken = authToken;

        // TODO: production
        var debugHandler = new HttpClientHandler();
        debugHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
        debugHandler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;

        _httpClient = new HttpClient(debugHandler);
    }

    public async Task<List<T>> Get<T>(System.String queryParams = "", System.String? endpointOverride = null) where T : GitLabModel
    {
        var endpoint = GitLabModel.ResolveEndpointAttribute(typeof(T));
        var uri = $"{_apiBase}{endpointOverride ?? endpoint}?private_token={_authToken}&{queryParams}";
        HttpResponseMessage? resp;
        try
        {
            resp = await _httpClient.GetAsync(uri);
        }
        catch (Exception e)
        {
            return new List<T>(); // TODO: better error handling than defaulting to an empty list
        }
        var result = await resp.Content.ReadFromJsonAsync<List<T>>();
        return result.Select(m => m.WithClient(this) as T ?? throw new NotImplementedException()).ToList();
    }

    public async Task<ApiResult<T>> GetOne<T>(System.Int64 Id, System.String queryParams = "", System.String? endpointOverride = null) where T : GitLabModel
    {
        var endpoint = GitLabModel.ResolveEndpointAttribute(typeof(T));
        var uri = $"{_apiBase}{endpointOverride ?? endpoint}/{Id}?private_token={_authToken}&{queryParams}";
        HttpResponseMessage? resp;
        try
        {
            resp = await _httpClient.GetAsync(uri);
        }
        catch (Exception e)
        {
            return ApiResult<T>.MakeError(e.Message);
        }
        if (resp.StatusCode != HttpStatusCode.OK)
        {
            var error = await resp.Content.ReadAsStringAsync();
            return ApiResult<T>.MakeError(error);
        }
        var result = await resp.Content.ReadFromJsonAsync<T>();
        var apiResult = result != null ? ApiResult<T>.MakeSuccess(result) : ApiResult<T>.MakeError("couldn't deserialize response");
        if (apiResult.Ok)
            apiResult.WithClient(this);
        return apiResult;
    }

    public async Task<ApiResult<T>> Post<T, D>(D data, System.String? endpoint = null) where T : GitLabModel
    {
        if (endpoint == null)
            endpoint = GitLabModel.ResolveEndpointAttribute(typeof(T));
        var uri = $"{_apiBase}{endpoint}?private_token={_authToken}";
        HttpResponseMessage? resp;
        try
        {
            resp = await _httpClient.PostAsJsonAsync<D>(uri, data);
        }
        catch (Exception e)
        {
            return ApiResult<T>.MakeError(e.Message);
        }
        if (resp.StatusCode != HttpStatusCode.Created)
        {
            var error = await resp.Content.ReadAsStringAsync();
            return ApiResult<T>.MakeError(error);
        }
        var result = await resp.Content.ReadFromJsonAsync<T>();
        var apiResult = result != null ? ApiResult<T>.MakeSuccess(result) : ApiResult<T>.MakeError("couldn't deserialize response");
        if (apiResult.Ok)
            apiResult.WithClient(this);
        return apiResult;
    }
}
