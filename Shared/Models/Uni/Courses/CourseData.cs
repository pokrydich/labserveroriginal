namespace LabServer.Shared.Models.Uni;

using System.Text.Json.Serialization;

[RESTData("/api/rest/courses", creatable: true, updatable: true)]
public class CourseData : DataModel
{
    [DataProperty("#")]
    [JsonIgnore]
    public System.Int64 ShowId => Id;
    [DataProperty("Name", editable: true, forCreate: true)]
    public System.String Name { get; set; } = System.String.Empty;
    [DataProperty("GitLab name", editable: true)]
    public System.String GitLabName { get; set; } = System.String.Empty;
    public IList<CourseLabData>? CourseLabs { get; set; }

    public override void Update(DataModel other)
    {
        CourseData update = other as CourseData ?? throw new NotImplementedException();
        Name = update.Name;
        GitLabName = update.GitLabName;
    }
}