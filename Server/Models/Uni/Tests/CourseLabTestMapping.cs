namespace LabServer.Server.Models.Uni;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

using LabServer.Shared.Models.Uni;
using LabServer.Shared.Models;

[Index(nameof(Id), IsUnique = true)]
[Index(nameof(CourseLabId), nameof(TestId), IsUnique = true)]
public class CourseLabTestMapping : LazyLoadedModel, IDataModel<CourseLabTestData>
{
    public CourseLabTestMapping() { }
    public CourseLabTestMapping(ILazyLoader lazyLoader) : base(lazyLoader) { }

    // mapping properties
    private CourseLabModel? _courseLab;
    public System.Int64 CourseLabId { get; set; }
    public CourseLabModel CourseLab
    {
        get => Loader.Load(this, ref _courseLab) ?? throw new NotImplementedException();
        set => _courseLab = value;
    }
    private TestModel? _test;
    public System.Int64 TestId { get; set; }
    public TestModel Test
    {
        get => Loader.Load(this, ref _test) ?? throw new NotImplementedException();
        set => _test = value;
    }

    // navigation properties
    private ICollection<TestRunModel>? _testRuns;
    public ICollection<TestRunModel> TestRuns
    {
        get => Loader.Load(this, ref _testRuns) ?? throw new NotImplementedException();
        set => _testRuns = value;
    }

    // custom properties
    public System.Boolean Activated { get; set; } // switch test runs on / off for current lab

    public CourseLabTestData ToData(DataConversionOption conversionOption = DataConversionOption.Default) => conversionOption switch
    {
        DataConversionOption.Default => new CourseLabTestData
        {
            Id = Id,
            CourseLabId = CourseLabId,
            TestId = TestId,
            Activated = Activated
        },
        DataConversionOption.Parent => new CourseLabTestData
        {
            Id = Id,
            CourseLabId = CourseLabId,
            TestId = TestId,
            Activated = Activated,
            Test = Test.ToData()
        },
        DataConversionOption.Full => new CourseLabTestData
        {
            Id = Id,
            CourseLabId = CourseLabId,
            TestId = TestId,
            Activated = Activated,
            Test = Test.ToData()
        },
        _ => throw new NotImplementedException()
    };

    public void Update(CourseLabTestData data)
    {
        Activated = data.Activated;
    }
}