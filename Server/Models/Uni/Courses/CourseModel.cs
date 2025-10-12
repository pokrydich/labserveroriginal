namespace LabServer.Server.Models.Uni;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

using LabServer.Shared.Models.Uni;
using LabServer.Shared.Models;
using LabServer.Server.Helpers;

[Index(nameof(Id), IsUnique = true)]
[Index(nameof(Name), IsUnique = true)]
public class CourseModel : LazyLoadedModel, IDataModel<CourseData>, ICreatable<CourseData>
{
    public CourseModel() { }
    public CourseModel(ILazyLoader lazyLoader) : base(lazyLoader) { }
    public System.String Name { get; set; } = System.String.Empty;
    public System.String GitLabName { get; set; } = System.String.Empty;
    private ICollection<CourseLabModel>? _courseLabs;
    public ICollection<CourseLabModel> CourseLabs
    {
        get => Loader.Load(this, ref _courseLabs) ?? throw new NotImplementedException();
        set => _courseLabs = value;
    }
    private ICollection<GroupCourseMapping>? _groupsMapping;
    public ICollection<GroupCourseMapping> GroupsMapping
    {
        get => Loader.Load(this, ref _groupsMapping) ?? throw new NotImplementedException();
        set => _groupsMapping = value;
    }

    public CourseData ToData(DataConversionOption conversionOption = DataConversionOption.Default) => conversionOption switch
    {
        DataConversionOption.Default => new CourseData
        {
            Id = Id,
            Name = Name,
            GitLabName = GitLabName
        },
        DataConversionOption.Full => new CourseData
        {
            Id = Id,
            Name = Name,
            GitLabName = GitLabName,
            CourseLabs = CourseLabs.ToData<CourseLabModel, CourseLabData>().ToList()
        },
        _ => throw new NotImplementedException()
    };

    public void Update(CourseData data)
    {
        Name = data.Name;
        GitLabName = data.GitLabName;
    }

    public void Init(CourseData data)
    {
        Name = data.Name;
        GitLabName = GitLabNameTransformer.Transliterate(data.Name);
    }
}