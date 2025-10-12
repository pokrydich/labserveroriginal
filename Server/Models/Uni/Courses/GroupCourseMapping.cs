namespace LabServer.Server.Models.Uni;

using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

using LabServer.Server.Data;
using LabServer.Shared.Models.Uni;

using GitLab.Models.Group;
using GitLab.Models;

using IndexAttribute = Microsoft.EntityFrameworkCore.IndexAttribute;
using LabServer.Shared.Models;

[Index(nameof(Id), IsUnique = true)]
[Index(nameof(GroupId), nameof(CourseId), IsUnique = true)]
public class GroupCourseMapping : GitLabIntegratedModel, IDataModel<GroupCourseData>
{
    public GroupCourseMapping() { }
    public GroupCourseMapping(ILazyLoader lazyLoader, LabsContext context) : base(lazyLoader, context) { }
    public GroupModel? _group;
    public System.Int64 GroupId { get; set; }
    public GroupModel Group
    {
        get => Loader.Load(this, ref _group) ?? throw new NotImplementedException();
        set => _group = value;
    }
    private CourseModel? _course;
    public System.Int64 CourseId { get; set; }
    public CourseModel Course
    {
        get => Loader.Load(this, ref _course) ?? throw new NotImplementedException();
        set => _course = value;
    }
    private ICollection<GroupCourseLabMapping>? _groupCourseLabs;
    public ICollection<GroupCourseLabMapping> GroupCourseLabs
    {
        get => Loader.Load(this, ref _groupCourseLabs) ?? throw new NotImplementedException();
        set => _groupCourseLabs = value;
    }
    public System.Int64? GitLabGroupId { get; set; }
    [NotMapped]
    [JsonIgnore]
    public GitLabGroup GitLabGroup => GitLabGroupId != null ? GitLab.GetOne<GitLabGroup>(GitLabGroupId.Value)
                        .GetAwaiter()
                        .GetResult().Result // TODO: may throw error
                            : throw new NotImplementedException("student does not hava a gitlab account initialized");

    public async Task<ApiResult<GitLabGroup>> SyncWithGitLab()
    {
        if (Group.GitLabGroupId == null)
            throw new NotImplementedException(); // shouldn't sync courses for groups that are not synced themselves
        if (GitLabGroupId == null)
        {
            var gitLabResult = await GitLab.CreateGroup(new CreateGroupRequest
            {
                Name = Course.GitLabName,
                Path = Course.GitLabName,
                ParentId = Group.GitLabGroupId
            });
            if (gitLabResult.Ok)
                GitLabGroupId = gitLabResult.Result.Id;
            return gitLabResult;
        }
        // TODO: update
        return await GitLab.GetOne<GitLabGroup>(GitLabGroupId.Value);
    }

    public GroupCourseData ToData(DataConversionOption conversionOption = DataConversionOption.Default) => conversionOption switch
    {
        DataConversionOption.Default => new GroupCourseData
        {
            Id = Id,
            GroupId = GroupId,
            CourseId = CourseId,
            GitLabGroupId = GitLabGroupId
        },
        DataConversionOption.Mapping => new GroupCourseData
        {
            Id = Id,
            GroupId = GroupId,
            CourseId = CourseId,
            GitLabGroupId = GitLabGroupId,
            Course = Course.ToData()
        },
        DataConversionOption.Full => new GroupCourseData
        {
            Id = Id,
            GroupId = GroupId,
            CourseId = CourseId,
            GitLabGroupId = GitLabGroupId,
            Course = Course.ToData(DataConversionOption.Full),
            Group = Group.ToData(DataConversionOption.Full),
            GroupLabs = GroupCourseLabs.ToData<GroupCourseLabMapping, GroupCourseLabData>(DataConversionOption.Full).ToList()
        },
        _ => throw new NotImplementedException()
    };

    public void Update(GroupCourseData data)
    {
    }
}