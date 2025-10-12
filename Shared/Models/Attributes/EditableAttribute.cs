namespace LabServer.Shared.Models;

public class RESTDataAttribute : Attribute
{
    private System.String _endpoint;
    private System.Boolean _creatable;
    private System.Boolean _updatable;
    public RESTDataAttribute(System.String endpoint, System.Boolean creatable = false,
        System.Boolean updatable = false)
    {
        _endpoint = endpoint;
        _creatable = creatable;
        _updatable = updatable;
    }
    public System.String Endpoint => _endpoint;
    public System.Boolean Creatable => _creatable;
    public System.Boolean Updatable => _updatable;
}