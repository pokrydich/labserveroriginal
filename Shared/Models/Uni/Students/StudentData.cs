namespace LabServer.Shared.Models.Uni;

using System.Text.Json.Serialization;

using GitLab.Models.User;

[RESTData("/api/rest/students", creatable: true, updatable: true)]
public class StudentData : DataModel
{
    [DataProperty("#")]
    public System.Int64 ShowId => Id;
    public System.Int64 GroupId { get; set; }
    public GroupData? Group { get; set; }
    [DataProperty("Name", editable: true, forCreate: true)]
    public System.String Name { get; set; } = System.String.Empty;
    [DataProperty("Username", editable: true)]
    public System.String Username { get; set; } = System.String.Empty;
    [DataProperty("E-mail", editable: true, forCreate: true)]
    public System.String Email { get; set; } = System.String.Empty;
    [DataProperty("Password")]
    public System.String? InitialPassword { get; set; }
    public System.String? DashboardToken { get; set; }
    public System.Int64? GitLabUserId { get; set; }
    [JsonIgnore]
    [DataProperty("GitLab-synced")]
    public System.Boolean GitLabSynced => GitLabUserId != null;
    public GitLabUser? GitLabUser { get; set; }
    public IEnumerable<StudentLabData>? LabsData { get; set; }

    public override void Update(DataModel other)
    {
        StudentData update = other as StudentData ?? throw new NotImplementedException();
        Name = update.Name;
        Username = update.Username;
        Email = update.Email;
        InitialPassword = update.InitialPassword;
        GitLabUserId = update.GitLabUserId;
        GitLabUser = update.GitLabUser;
    }
}