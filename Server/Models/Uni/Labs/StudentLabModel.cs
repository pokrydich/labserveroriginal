namespace LabServer.Server.Models.Uni;

using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Microsoft.EntityFrameworkCore.Infrastructure;

using LabServer.Server.Data;
using LabServer.Shared.Models.Uni;

using GitLab.Models;
using GitLab.Models.Project;

using IndexAttribute = Microsoft.EntityFrameworkCore.IndexAttribute;
using LabServer.Shared.Models;

/// <summary>
/// AKA Student to CourseLab mapping with additional data
/// </summary>
[Index(nameof(Id), IsUnique = true)]
[Index(nameof(GroupCourseLabId), nameof(StudentId), IsUnique = true)]
public class StudentLabModel : GitLabIntegratedModel, IDataModel<StudentLabData>, IRestricted
{
    public StudentLabModel() { }
    public StudentLabModel(ILazyLoader lazyLoader, LabsContext context) : base(lazyLoader, context) { }
    public DateTime OpenedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public StudentLabStatus Status { get; set; }
    public System.String Notes { get; set; } = System.String.Empty; // note's on student's progress
    public System.Int64? GitLabProjectId { get; set; }
    [NotMapped]
    [JsonIgnore]
    public GitLabProject? GitLabProject => GitLabProjectId != null ? GitLab.GetOne<GitLabProject>(GitLabProjectId.Value)
                        .GetAwaiter()
                        .GetResult().Result // TODO: may throw error
                            : null;
    private GroupCourseLabMapping? _groupCourseLab;
    public System.Int64 GroupCourseLabId { get; set; }
    public GroupCourseLabMapping GroupCourseLab
    {
        get => Loader.Load(this, ref _groupCourseLab) ?? throw new NotImplementedException();
        set => _groupCourseLab = value;
    }
    private StudentModel? _student;
    public System.Int64 StudentId { get; set; }
    public StudentModel Student
    {
        get => Loader.Load(this, ref _student) ?? throw new NotImplementedException();
        set => _student = value;
    }
    private ICollection<StudentLabSubmissionModel>? _labSubmissions;
    public ICollection<StudentLabSubmissionModel> LabSubmissions
    {
        get => Loader.Load(this, ref _labSubmissions) ?? throw new NotImplementedException();
        set => _labSubmissions = value;
    }

    public IEnumerable<long> AllowedIDs => Student.AllowedIDs;

    public async Task<ApiResult<GitLabProject>> SyncWithGitLab()
    {
        if (Student.GitLabUserId == null)
            return ApiResult<GitLabProject>.MakeError($"student '{Student.Name}' should be syncronized with GitLab first");

        ApiResult<GitLabProject>? result = null;
        if (GitLabProjectId == null)
        {
            var gitLabResult = await GitLab.CreateProject(new CreateProjectRequest
            {
                Name = $"{Student.Username}_{GroupCourseLab.CourseLab.GitLabName}",
                NamespaceId = GroupCourseLab.GitLabGroupId,
                InitializeWithReadme = true,
                DefaultBranch = "main"
            });
            if (gitLabResult.Ok)
            {
                GitLabProjectId = gitLabResult.Result.Id;
            }
            result = gitLabResult;
        }
        var addMemberResult = await result.Result.AddMember(Student.GitLabUser, PorjectAccessLevel.Developper);
        if (addMemberResult.Ok)
            return await GitLab.GetOne<GitLabProject>(GitLabProjectId.Value);
        else
            return ApiResult<GitLabProject>.MakeError("couldn't add student's gitlab user to project");
    }

    public StudentLabData ToData(DataConversionOption conversionOption = DataConversionOption.Default) => conversionOption switch
    {
        DataConversionOption.Default => new StudentLabData
        {
            Id = Id,
            GroupCourseLabId = GroupCourseLabId,
            StudentId = StudentId,
            OpenedDate = OpenedDate,
            CompletedDate = CompletedDate,
            Status = Status,
            Notes = Notes,
            GitLabProjectId = GitLabProjectId,
        },
        DataConversionOption.Full => new StudentLabData
        {
            Id = Id,
            GroupCourseLabId = GroupCourseLabId,
            StudentId = StudentId,
            OpenedDate = OpenedDate,
            CompletedDate = CompletedDate,
            Status = Status,
            Notes = Notes,
            GitLabProjectId = GitLabProjectId,
            GitLabProject = GitLabProject,
            LabSubmissions = LabSubmissions.ToData<StudentLabSubmissionModel, StudentLabSubmissionData>().ToList(),
            GroupCourseLab = GroupCourseLab.ToData(DataConversionOption.Parent),
            Student = Student.ToData(DataConversionOption.Parent)
        },
        DataConversionOption.Children => new StudentLabData
        {
            Id = Id,
            GroupCourseLabId = GroupCourseLabId,
            StudentId = StudentId,
            OpenedDate = OpenedDate,
            CompletedDate = CompletedDate,
            Status = Status,
            Notes = Notes,
            GitLabProjectId = GitLabProjectId,
            GitLabProject = GitLabProject,
            LabSubmissions = LabSubmissions.ToData<StudentLabSubmissionModel, StudentLabSubmissionData>().ToList(),
        },
        _ => throw new NotImplementedException()
    };

    public void Update(StudentLabData data)
    {
        Status = data.Status;
    }
}