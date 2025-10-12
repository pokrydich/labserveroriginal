namespace LabServer.Server.Tests.Services;

using LabServer.Server.Models.Uni;
using LabServer.Server.Service;
using LabServer.Server.Tests.TestFixtures;

public class DBStorageTests
{
    [Fact]
    public async Task GetAll_ReturnsEntities()
    {
        var gitLabMock = new Mock<IGitLab>();
        await using var context = InMemoryDbContextFactory.CreateContext(Guid.NewGuid().ToString(), gitLabMock.Object);
        context.Courses.AddRange(
            new CourseModel { Name = "Math", GitLabName = "math" },
            new CourseModel { Name = "Physics", GitLabName = "physics" });
        await context.SaveChangesAsync();

        var storage = new DBStorage<CourseModel>(context);

        var result = storage.GetAll().ToList();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetById_ReturnsMatchOrNull()
    {
        var gitLabMock = new Mock<IGitLab>();
        await using var context = InMemoryDbContextFactory.CreateContext(Guid.NewGuid().ToString(), gitLabMock.Object);
        var entity = new CourseModel { Name = "Algorithms", GitLabName = "algorithms" };
        context.Courses.Add(entity);
        await context.SaveChangesAsync();

        var storage = new DBStorage<CourseModel>(context);

        var stored = await storage.GetById(entity.Id);
        Assert.NotNull(stored);
        Assert.Null(await storage.GetById(999));
    }

    [Fact]
    public async Task AddAndApplyChanges_PersistsEntity()
    {
        var gitLabMock = new Mock<IGitLab>();
        await using var context = InMemoryDbContextFactory.CreateContext(Guid.NewGuid().ToString(), gitLabMock.Object);
        var storage = new DBStorage<CourseModel>(context);

        await storage.Add(new CourseModel { Name = "Databases", GitLabName = "databases" });
        await storage.ApplyChangesAsync();

        Assert.Equal(1, await context.Courses.CountAsync());
    }
}
