namespace LabServer.Shared.Models.Uni;

using System.Text.Json.Serialization;

[RESTData("/api/rest/tests", creatable: true, updatable: true)]
public class TestData : DataModel
{
    [DataProperty("#")]
    [JsonIgnore]
    public System.Int64 ShowId => Id;
    [DataProperty("Name", editable: true, forCreate: true)]
    public System.String Name { get; set; } = System.String.Empty;
    [DataProperty("Server URL", editable: true, forCreate: true)]
    public System.String TestServerUrl { get; set; } = System.String.Empty;

    public override void Update(DataModel other)
    {
        TestData update = other as TestData ?? throw new NotImplementedException();
        Name = update.Name;
        TestServerUrl = update.TestServerUrl;
    }
}