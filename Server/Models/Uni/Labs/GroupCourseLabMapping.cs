namespace LabServer.Server.Models.Uni;

using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Microsoft.EntityFrameworkCore.Infrastructure;

using LabServer.Server.Data;
using LabServer.Shared.Models.Uni;

using GitLab.Models.Group;
using GitLab.Models;

using IndexAttribute = Microsoft.EntityFrameworkCore.IndexAttribute;
using LabServer.Shared.Models;

[Index(nameof(Id), IsUnique = true)]
[Index(nameof(GroupCourseId), nameof(CourseLabId), IsUnique = true)]
public class GroupCourseLabMapping : GitLabIntegratedModel, IRestricted, IDataModel<GroupCourseLabData>
{
    public GroupCourseLabMapping() { }
    public GroupCourseLabMapping(ILazyLoader lazyLoader, LabsContext context) : base(lazyLoader, context) { }
    public DateTime StartDate { get; set; }
    public DateTime? DeadlineDate { get; set; }
    public GroupCourseMapping? _groupCourse;
    public System.Int64 GroupCourseId { get; set; }
    public GroupCourseMapping GroupCourse
    {
        get => Loader.Load(this, ref _groupCourse) ?? throw new NotImplementedException();
        set => _groupCourse = value;
    }
    private CourseLabModel? _courseLab;
    public System.Int64 CourseLabId { get; set; }
    public CourseLabModel CourseLab
    {
        get => Loader.Load(this, ref _courseLab) ?? throw new NotImplementedException();
        set => _courseLab = value;
    }
    private ICollection<StudentLabModel>? _labsForStudents;
    public ICollection<StudentLabModel> LabsForStudents
    {
        get => Loader.Load(this, ref _labsForStudents) ?? throw new NotImplementedException();
        set => _labsForStudents = value;
    }
    public System.Int64? GitLabGroupId { get; set; }
    [NotMapped]
    [JsonIgnore]
    public GitLabGroup GitLabGroup => GitLabGroupId != null ? GitLab.GetOne<GitLabGroup>(GitLabGroupId.Value)
                        .GetAwaiter()
                        .GetResult().Result // TODO: may throw error
                            : throw new NotImplementedException("student does not hava a gitlab account initialized");

    public IEnumerable<System.Int64> AllowedIDs => GroupCourse.Group.AllowedIDs;

    public async Task<ApiResult<GitLabGroup>> SyncWithGitLab()
    {
        if (GitLabGroupId == null)
        {
            var gitLabResult = await GitLab.CreateGroup(new CreateGroupRequest
            {
                Name = CourseLab.GitLabName,
                Path = CourseLab.GitLabName,
                ParentId = GroupCourse.GitLabGroupId
            });
            if (gitLabResult.Ok)
                GitLabGroupId = gitLabResult.Result.Id;
            return gitLabResult;
        }
        // TODO: update
        return await GitLab.GetOne<GitLabGroup>(GitLabGroupId.Value);
    }

    public GroupCourseLabData ToData(DataConversionOption conversionOption = DataConversionOption.Default) => conversionOption switch
    {
        DataConversionOption.Default => new GroupCourseLabData
        {
            Id = Id,
            StartDate = StartDate,
            DeadlineDate = DeadlineDate,
            GroupCourseId = GroupCourseId,
            CourseLabId = CourseLabId,
            GitLabGroupId = GitLabGroupId
        },
        DataConversionOption.Parent => new GroupCourseLabData
        {
            Id = Id,
            StartDate = StartDate,
            DeadlineDate = DeadlineDate,
            GroupCourseId = GroupCourseId,
            CourseLabId = CourseLabId,
            GitLabGroupId = GitLabGroupId,
            CourseLab = CourseLab.ToData(DataConversionOption.Parent)
        },
        DataConversionOption.Full => new GroupCourseLabData
        {
            Id = Id,
            StartDate = StartDate,
            DeadlineDate = DeadlineDate,
            GroupCourseId = GroupCourseId,
            CourseLabId = CourseLabId,
            GitLabGroupId = GitLabGroupId,
            StudentLabs = LabsForStudents.ToData<StudentLabModel, StudentLabData>(DataConversionOption.Children).ToList()
        },
        _ => throw new NotImplementedException()
    };

    public void Update(GroupCourseLabData data)
    {
        DeadlineDate = data.DeadlineDate;
    }
}