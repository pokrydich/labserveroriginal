namespace LabServer.Server.Models.Uni;

using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Microsoft.EntityFrameworkCore.Infrastructure;

using LabServer.Server.Data;
using LabServer.Shared.Models.Uni;

using GitLab.Models.MergeRequest;

using IndexAttribute = Microsoft.EntityFrameworkCore.IndexAttribute;
using LabServer.Shared.Models;

[Index(nameof(Id), IsUnique = true)]
[Index(nameof(GitLabMergeRequestId), IsUnique = true)]
public class StudentLabSubmissionModel : GitLabIntegratedModel, IRestricted, IDataModel<StudentLabSubmissionData>
{
    public StudentLabSubmissionModel() { }
    public StudentLabSubmissionModel(ILazyLoader lazyLoader, LabsContext context) : base(lazyLoader, context) { }
    public System.Int64? GitLabMergeRequestId { get; set; }
    public System.Int64? GitLabMergeRequestIid { get; set; }
    public StudentLabSubmissionStatus Status { get; set; }
    public DateTime SubmittedDate { get; set; }
    [NotMapped]
    [JsonIgnore]
    public GitLabMergeRequest? GitLabMergeRequest => GitLabMergeRequestIid != null ? GitLab.GetOne<GitLabMergeRequest>(GitLabMergeRequestIid.Value, endpointOverride: $"/projects/{StudentLab.GitLabProjectId}/merge_requests")
                        .GetAwaiter()
                        .GetResult().Result // TODO: may throw error
                            : null;
    
    private StudentLabModel? _studentLab;
    public System.Int64 StudentLabId { get; set; }
    public StudentLabModel StudentLab
    {
        get => Loader.Load(this, ref _studentLab) ?? throw new NotImplementedException();
        set => _studentLab = value;
    }
    // navigation properties
    private ICollection<TestRunModel>? _testRuns;
    public ICollection<TestRunModel> TestRuns
    {
        get => Loader.Load(this, ref _testRuns) ?? throw new NotImplementedException();
        set => _testRuns = value;
    }

    public IEnumerable<long> AllowedIDs => StudentLab.AllowedIDs;

    public StudentLabSubmissionData ToData(DataConversionOption conversionOption = DataConversionOption.Default) => conversionOption switch
    {
        DataConversionOption.Default => new StudentLabSubmissionData
        {
            Id = Id,
            StudentLabId = StudentLabId,
            GitLabMergeRequestId = GitLabMergeRequestId,
            GitLabMergeRequestIid = GitLabMergeRequestIid,
            Status = Status,
            SubmittedDate = SubmittedDate,
            GitLabMergeRequest = GitLabMergeRequest
        },
        DataConversionOption.Parent => new StudentLabSubmissionData
        {
            Id = Id,
            StudentLabId = StudentLabId,
            GitLabMergeRequestId = GitLabMergeRequestId,
            GitLabMergeRequestIid = GitLabMergeRequestIid,
            Status = Status,
            SubmittedDate = SubmittedDate,
            GitLabMergeRequest = GitLabMergeRequest
        }
    };

    public void Update(StudentLabSubmissionData data)
    {
    }
}