using System.Reflection;

namespace LabServer.Shared.Models;

public enum DataConversionOption
{
    Default = 0,
    Parent,
    Children,
    Mapping,
    Full,
    GitLab
}

public abstract class DataModel
{
    public System.Int64 Id { get; set; }
    public abstract void Update(DataModel other);
}

public class DataProperty
{
    private DataPropertyAttribute _attribute;
    private PropertyInfo _propInfo;
    public DataProperty(DataPropertyAttribute attribute, PropertyInfo propInfo)
    {
        _attribute = attribute;
        _propInfo = propInfo;
    }

    public System.String DataTitle => _attribute.Title;
    public System.Boolean IsEditable => _attribute.Editable;
    public System.Boolean ForCreate => _attribute.ForCreate;
    public Type PropertyType => _propInfo.PropertyType;
    public System.Object? GetValue(System.Object isntance) => _propInfo.GetValue(isntance);
    public void SetValue(System.Object instance, System.Object value) => _propInfo.SetValue(instance, value);
}

public static class DataModelExtentions
{
    public static List<DataProperty> GetPropertiesList<D>() where D : DataModel
    {
        var result = new List<DataProperty>();
        foreach (var prop in typeof(D).GetProperties())
        {
            var attribute = prop.GetCustomAttributes(typeof(DataPropertyAttribute), false).FirstOrDefault();
            if (attribute == null)
                continue;
            var dataProperty = attribute as DataPropertyAttribute ?? throw new NotImplementedException();
            result.Add(new DataProperty(dataProperty, prop));
        }
        return result;
    }
}