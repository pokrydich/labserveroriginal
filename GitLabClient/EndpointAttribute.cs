namespace GitLab;

public class GetEndpointAttribute : Attribute
{
    private System.String _endpoint;
    public System.String Endpoint => _endpoint;
    public GetEndpointAttribute(System.String endpoint)
    {
        _endpoint = endpoint;
    }
}

public class PostEndpointAttribute : Attribute
{
    private System.String _format;
    private System.Type[] _paramTypes;
    public PostEndpointAttribute(System.String format, System.Type[]? paramTypes = null)
    {
        _format = format;
        _paramTypes = paramTypes ?? new System.Type[0];
    }

    private System.Boolean _checkParams(System.Object[] parameters)
    {
        if (parameters.Length != _paramTypes.Length)
            throw new NotImplementedException();
        for (System.Int32 i = 0; i < parameters.Length; ++i)
            if (parameters[i].GetType() != _paramTypes[i])
                throw new NotImplementedException();
        return true;
    }

    public System.String FormatEndpoint(System.Object[]? parameters = null)
    {
        parameters ??= new System.Object[0];
        if (!_checkParams(parameters))
            throw new NotImplementedException();
        return System.String.Format(_format, parameters);
    }
}