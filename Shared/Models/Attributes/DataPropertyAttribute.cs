namespace LabServer.Shared.Models;

public class DataPropertyAttribute : Attribute
{
    private System.String _title;
    private System.Boolean _editable;
    private System.Boolean _forCreate;
    public DataPropertyAttribute(System.String title, System.Boolean editable = false, System.Boolean forCreate = false)
    {
        _title = title;
        _editable = editable;
        _forCreate = forCreate;
    }
    public System.String Title => _title;
    public System.Boolean Editable => _editable;
    public System.Boolean ForCreate => _forCreate;
}