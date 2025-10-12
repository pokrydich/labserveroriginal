namespace LabServer.Server.Models.Uni;

using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


using Microsoft.EntityFrameworkCore.Infrastructure;

using LabServer.Server.Data;
using LabServer.Shared.Models.Uni;
using LabServer.Server.Service;

using GitLab.Models.Group;
using GitLab.Models;

using IndexAttribute = Microsoft.EntityFrameworkCore.IndexAttribute;
using LabServer.Shared.Models;
using LabServer.Server.Helpers;

[Index(nameof(Id), IsUnique = true)]
[Index(nameof(Name), IsUnique = true)]
public class GroupModel : PersonalModel<GroupProfessorMapping, GroupModel>, 
                            ICreatable<GroupData>,
                            IGitIntegrated
{
    public GroupModel() { }
    public void Init(GroupData data)
    {
        Name = data.Name;
        GitLabName = GitLabNameTransformer.Transliterate(data.Name);
    }
    private IGitLab? _gitLab;
    public IGitLab GitLab => _gitLab ?? throw new NotImplementedException();
    public void InjectGitLab(IGitLab gitLab) => _gitLab = gitLab;
    private GroupModel(ILazyLoader lazyLoader, LabsContext context) : base(lazyLoader) 
    {
        _gitLab = context.GitLabService;
    }
    public System.String Name { get; set; } = System.String.Empty;
    public System.String GitLabName { get; set; } = System.String.Empty;
    public System.Int64? GitLabGroupId { get; set; } // maps UNI group to GitLabGroup
    [NotMapped]
    [JsonIgnore]
    public GitLabGroup? GitLabGroup => GitLabGroupId != null ? GitLab.GetOne<GitLabGroup>(GitLabGroupId.Value)
                        .GetAwaiter()
                        .GetResult().Result // TODO: may throw error
                            : null;
    private ICollection<StudentModel>? _students;
    public ICollection<StudentModel> Students
    {
        get => Loader.Load(this, ref _students) ?? throw new NotImplementedException();
        set => _students = value;
    }
    private ICollection<GroupCourseMapping>? _coursesMapping;
    public ICollection<GroupCourseMapping> CoursesMapping
    {
        get => Loader.Load(this, ref _coursesMapping) ?? throw new NotImplementedException();
        set => _coursesMapping = value;
    }

    public async Task<ApiResult<GitLabGroup>> SyncWithGitLab()
    {
        if (GitLabGroupId == null)
        {
            var gitLabResult = await GitLab.CreateGroup(new CreateGroupRequest
            {
                Name = GitLabName,
                Path = GitLabName
            });
            if (gitLabResult.Ok)
                GitLabGroupId = gitLabResult.Result.Id;
            return gitLabResult;
        }
        // TODO: update
        return await GitLab.GetOne<GitLabGroup>(GitLabGroupId.Value);
    }
    public GroupData ToData(DataConversionOption conversionOption = DataConversionOption.Default) => conversionOption switch
    {
        DataConversionOption.Default => new GroupData
        {
            Id = Id,
            Name = Name,
            GitLabName = GitLabName,
            GitLabGroupId = GitLabGroupId
        },
        DataConversionOption.GitLab => new GroupData
        {
            Id = Id,
            Name = Name,
            GitLabName = GitLabName,
            GitLabGroupId = GitLabGroupId,
            GitLabGroup = GitLabGroup
        },
        DataConversionOption.Full => new GroupData
        {
            Id = Id,
            Name = Name,
            GitLabName = GitLabName,
            GitLabGroupId = GitLabGroupId,
            Students = Students.ToData<StudentModel, StudentData>()
        },
        _ => throw new NotImplementedException()
    };

    public void Update(GroupData data)
    {
        Name = data.Name;
        GitLabName = data.GitLabName;
    }
}