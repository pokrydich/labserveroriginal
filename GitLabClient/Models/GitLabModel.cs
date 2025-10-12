namespace GitLab.Models;

using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class GitLabModel
{
    [JsonIgnore]
    private GitLabClient? _createdClient;
    public GitLabModel WithClient(GitLabClient createdClient)
    {
        _createdClient = createdClient;
        return this;
    }
    [JsonIgnore]
    [NotMapped]
    protected GitLabClient Client => _createdClient ?? throw new NotImplementedException();
    public static System.String ResolveEndpointAttribute(Type type)
    {
        var getEndpoint = type.GetCustomAttributes(typeof(GetEndpointAttribute), false).Single() as GetEndpointAttribute ?? throw new NotImplementedException();
        return getEndpoint.Endpoint;
    }
}