namespace LabServer.Server.Models.Uni;

using Microsoft.EntityFrameworkCore.Infrastructure;

using LabServer.Shared.Models.Uni;

using IndexAttribute = Microsoft.EntityFrameworkCore.IndexAttribute;
using System.Collections.Generic;
using LabServer.Shared.Models;

[Index(nameof(Id), IsUnique = true)]
[Index(nameof(CourseLabTestMappingId), nameof(StudentLabSubmissionId), IsUnique = true)] // it's a mapping
public class TestRunModel : LazyLoadedModel, IDataModel<TestRunData>, IRestricted
{
    public TestRunModel() { }
    public TestRunModel(ILazyLoader lazyLoader) : base(lazyLoader) { }
    // mapping properties
    private CourseLabTestMapping? _courseLabTest;
    public System.Int64 CourseLabTestMappingId { get; set; }
    public CourseLabTestMapping CourseLabTest
    {
        get => Loader.Load(this, ref _courseLabTest) ?? throw new NotImplementedException();
        set => _courseLabTest = value;
    }
    private StudentLabSubmissionModel? _studentLabSubmission;
    public System.Int64 StudentLabSubmissionId { get; set; }
    public StudentLabSubmissionModel StudentLabSubmission
    {
        get => Loader.Load(this, ref _studentLabSubmission) ?? throw new NotImplementedException();
        set => _studentLabSubmission = value;
    }

    // custom properties
    public TestRunState State { get; set; }
    public DateTime ScheduledDate { get; set; }
    public System.Boolean? Success { get; set; } // stores test run boolean result (when the test is in State.Completed)
    public System.String? Message { get; set; } // stores test result description message (for example, the reasons of failure) (State.Completed required)

    public IEnumerable<long> AllowedIDs => StudentLabSubmission.AllowedIDs;

    public TestRunData ToData(DataConversionOption conversionOption = DataConversionOption.Default)
    {
        throw new NotImplementedException();
    }

    public void Update(TestRunData data)
    {
        throw new NotImplementedException();
    }
}