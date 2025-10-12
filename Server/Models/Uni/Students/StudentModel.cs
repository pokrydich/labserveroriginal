namespace LabServer.Server.Models.Uni;

using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore.Infrastructure;

using LabServer.Server.Data;
using LabServer.Shared.Models.Uni;

using GitLab.Models.User;
using GitLab.Models;
using LabServer.Server.Helpers;

using IndexAttribute = Microsoft.EntityFrameworkCore.IndexAttribute;
using LabServer.Shared.Models;

[Index(nameof(Id), IsUnique = true)]
[Index(nameof(Name), nameof(GroupId), IsUnique = true)]
[Index(nameof(Email), IsUnique = true)]
public class StudentModel : GitLabIntegratedModel, IDataModel<StudentData>, IRestricted
{
    public StudentModel() { }
    public StudentModel(ILazyLoader lazyLoader, LabsContext labsContext) : base(lazyLoader, labsContext) { }
    public System.String Name { get; set; } = System.String.Empty;
    public System.String Username { get; set; } = System.String.Empty;
    public System.String Email { get; set; } = System.String.Empty;
    public System.String? InitialPassword { get; set; }
    public System.String? DashboardToken { get; set; }
    public System.Int64? GitLabUserId { get; set; }
    [NotMapped]
    [JsonIgnore]
    public GitLabUser? GitLabUser => GitLabUserId != null ? GitLab.GetOne<GitLabUser>(GitLabUserId.Value)
                        .GetAwaiter()
                        .GetResult().Result // TODO: may throw
                            : null;
    private GroupModel? _group;
    public System.Int64 GroupId { get; set; }
    public GroupModel Group
    {
        get => Loader.Load(this, ref _group) ?? throw new NotImplementedException();
        set => _group = value;
    }
    private ICollection<StudentLabModel>? _labs;
    public ICollection<StudentLabModel> Labs
    {
        get => Loader.Load(this, ref _labs) ?? throw new NotImplementedException();
        set => _labs = value;
    }

    public IEnumerable<System.Int64> AllowedIDs => Group.AllowedIDs;

    public async Task<ApiResult<GitLabUser>> SyncWithGitLab(System.Boolean makeExternal = false, System.Boolean skipConfirmation = false)
    {
        if (GitLabUserId == null)
        {
            var initialPassword = RandomUtils.GetPassword(8);
            var gitLabResult = await GitLab.CreateUser(new CreateUserRequest
            {
                Username = Username,
                Name = Name,
                Email = Email,
                Password = initialPassword,
                IsExternal = makeExternal,
                SkipConfirmation = skipConfirmation
            });
            if (gitLabResult.Ok)
            {
                GitLabUserId = gitLabResult.Result.Id;
                InitialPassword = initialPassword;
                DashboardToken = RandomUtils.GetToken(64);
            }
            return gitLabResult;
        }
        // TODO: update
        return await GitLab.GetOne<GitLabUser>(GitLabUserId.Value);
    }

    public StudentData ToData(DataConversionOption conversionOption = DataConversionOption.Default) => conversionOption switch
    {
        DataConversionOption.Default => new StudentData
        {
            Id = Id,
            GroupId = GroupId,
            Name = Name,
            Username = Username,
            Email = Email,
            InitialPassword = InitialPassword,
            DashboardToken = DashboardToken,
            GitLabUserId = GitLabUserId,
            GitLabUser = GitLabUser
        },
        DataConversionOption.Parent => new StudentData
        {
            Id = Id,
            GroupId = GroupId,
            Name = Name,
            Username = Username,
            Email = Email,
            InitialPassword = InitialPassword,
            DashboardToken = DashboardToken,
            GitLabUserId = GitLabUserId,
            Group = Group.ToData()
        },
        DataConversionOption.Children => new StudentData
        {
            Id = Id,
            GroupId = GroupId,
            Name = Name,
            Username = Username,
            Email = Email,
            InitialPassword = InitialPassword,
            DashboardToken = DashboardToken,
            LabsData = Labs.ToData<StudentLabModel, StudentLabData>(DataConversionOption.Full)
        }
    };

    public void Update(StudentData data)
    {
        Name = data.Name;
        Username = data.Username;
        Email = data.Email;
    }
}