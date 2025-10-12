namespace LabServer.Server.Models.Uni;

using Microsoft.EntityFrameworkCore.Infrastructure;

using LabServer.Shared.Models;

using IndexAttribute = Microsoft.EntityFrameworkCore.IndexAttribute;
using LabServer.Shared.Models.Uni;

[Index(nameof(Id), IsUnique = true)]
[Index(nameof(Name), IsUnique = true)]
public class TestModel : LazyLoadedModel, IDataModel<TestData>, ICreatable<TestData>
{
    public TestModel() { }
    public TestModel(ILazyLoader lazyLoader) : base(lazyLoader) { }
    public System.String Name { get; set; } = System.String.Empty;
    public System.String TestServerUrl { get; set; } = System.String.Empty;
    private ICollection<CourseLabTestMapping>? _labMapping;
    public ICollection<CourseLabTestMapping> LabMapping
    {
        get => Loader.Load(this, ref _labMapping) ?? throw new NotImplementedException();
        set => _labMapping = value;
    }

    public TestData ToData(DataConversionOption conversionOption = DataConversionOption.Default) => new TestData
    {
        Id = Id,
        Name = Name,
        TestServerUrl = TestServerUrl
    };

    public void Update(TestData data)
    {
        Name = data.Name;
        TestServerUrl = data.TestServerUrl;
    }

    public void Init(TestData data)
    {
        Name = data.Name;
        TestServerUrl = data.TestServerUrl;
    }
}