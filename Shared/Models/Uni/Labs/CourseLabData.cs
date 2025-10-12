namespace LabServer.Shared.Models.Uni;

using System.Text.Json.Serialization;

[RESTData("/api/rest/courses/{}/labs", creatable: true, updatable: true)]
public class CourseLabData : DataModel
{
    [DataProperty("#")]
    [JsonIgnore]
    public System.Int64 ShowId => Id;
    public System.Int64 CourseId { get; set; }
    public CourseData? Course { get; set; }
    [DataProperty("Name", editable: true, forCreate: true)]
    public System.String Name { get; set; } = System.String.Empty;
    [DataProperty("GitLab name", editable: true)]
    public System.String GitLabName { get; set; } = System.String.Empty;
    public IList<CourseLabTestData>? LabTests { get; set; }

    public override void Update(DataModel other)
    {
        CourseLabData update = other as CourseLabData ?? throw new NotImplementedException();
        Name = update.Name;
        GitLabName = update.GitLabName;
    }
}