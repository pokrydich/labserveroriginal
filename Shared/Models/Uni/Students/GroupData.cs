namespace LabServer.Shared.Models.Uni;

using System.Text.Json.Serialization;

using GitLab.Models.Group;

[RESTData("/api/rest/groups", creatable: true, updatable: true)]
public class GroupData : DataModel
{
    [DataProperty("#")]
    [JsonIgnore]
    public System.Int64 ShowId => Id;
    [DataProperty("Name", editable: true, forCreate: true)]
    public System.String Name { get; set; } = System.String.Empty;
    [DataProperty("GitLab name", editable: true)]
    public System.String GitLabName { get; set; } = System.String.Empty;
    public System.Int64? GitLabGroupId { get; set; }
    [DataProperty("GitLab-synced")]
    [JsonIgnore]
    public System.Boolean GitLabSynced => GitLabGroupId != null;
    public GitLabGroup? GitLabGroup { get; set; }
    public IEnumerable<StudentData>? Students { get; set; }

    public override void Update(DataModel other)
    {
        GroupData update = other as GroupData ?? throw new NotImplementedException();
        Name = update.Name;
        GitLabName = update.GitLabName;
        GitLabGroupId = update.GitLabGroupId;
        GitLabGroup = update.GitLabGroup;
    }
}