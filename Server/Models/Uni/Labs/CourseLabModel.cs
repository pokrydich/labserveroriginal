namespace LabServer.Server.Models.Uni;

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;

using LabServer.Shared.Models.Uni;
using LabServer.Shared.Models;

[Index(nameof(Id), IsUnique = true)]
[Index(nameof(CourseId), nameof(Name), IsUnique = true)]
public class CourseLabModel : LazyLoadedModel, IDataModel<CourseLabData>
{
    public CourseLabModel() {}
    public CourseLabModel(ILazyLoader lazyLoader) : base(lazyLoader) {}
    public System.String Name { get; set; } = System.String.Empty;
    public System.String GitLabName { get; set; } = System.String.Empty;
    public System.String Description { get; set; } = System.String.Empty; // desc. for other users. TODO: Not used so far
    private CourseModel? _course;
    public System.Int64 CourseId { get; set; }
    public CourseModel Course
    {
        get => Loader.Load(this, ref _course) ?? throw new NotImplementedException();
        set => _course = value;
    }
    private ICollection<GroupCourseLabMapping>? _assignedGroups;
    public ICollection<GroupCourseLabMapping> AssignedGroups
    {
        get => Loader.Load(this, ref _assignedGroups) ?? throw new NotImplementedException();
        set => _assignedGroups = value;
    }
    private ICollection<CourseLabTestMapping>? _testMapping;
    public ICollection<CourseLabTestMapping> TestMapping
    {
        get => Loader.Load(this, ref _testMapping) ?? throw new NotImplementedException();
        set => _testMapping = value;
    }

    public CourseLabData ToData(DataConversionOption conversionOption = DataConversionOption.Default) => conversionOption switch
    {
        DataConversionOption.Default => new CourseLabData
        {
            Id = Id,
            CourseId = CourseId,
            Name = Name,
            GitLabName = GitLabName
        },
        DataConversionOption.Parent => new CourseLabData
        {
            Id = Id,
            CourseId = CourseId,
            Name = Name,
            GitLabName = GitLabName,
            Course = Course.ToData()
        },
        DataConversionOption.Full => new CourseLabData
        {
            Id = Id,
            CourseId = CourseId,
            Name = Name,
            GitLabName = GitLabName,
            LabTests = TestMapping.ToData<CourseLabTestMapping, CourseLabTestData>(DataConversionOption.Full).ToList()
        },
        _ => throw new NotImplementedException()
    };

    public void Update(CourseLabData data)
    {
        Name = data.Name;
        GitLabName = data.GitLabName;
    }
}