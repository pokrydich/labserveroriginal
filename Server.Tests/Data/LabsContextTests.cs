namespace LabServer.Server.Tests.Data;

using LabServer.Server.Data;
using LabServer.Server.Models.Uni;
using LabServer.Server.Service;
using LabServer.Server.Tests.TestFixtures;

public class LabsContextTests
{
    [Fact]
    public async Task Courses_CRUD_Succeeds()
    {
        var gitLabMock = new Mock<IGitLab>();
        await using var context = InMemoryDbContextFactory.CreateContext(Guid.NewGuid().ToString(), gitLabMock.Object);

        var course = new CourseModel
        {
            Name = "Programming 101",
            GitLabName = "programming_101"
        };

        context.Courses.Add(course);
        await context.SaveChangesAsync();

        var stored = await context.Courses.SingleAsync();
        Assert.Equal("Programming 101", stored.Name);

        stored.GitLabName = "programming-101";
        await context.SaveChangesAsync();

        var updated = await context.Courses.SingleAsync();
        Assert.Equal("programming-101", updated.GitLabName);
    }

    [Fact]
    public async Task GetStorage_ReturnsRegisteredDbSet()
    {
        var gitLabMock = new Mock<IGitLab>();
        await using var context = InMemoryDbContextFactory.CreateContext(Guid.NewGuid().ToString(), gitLabMock.Object);

        var storage = context.GetStorage<CourseModel>();
        storage.Add(new CourseModel
        {
            Name = "Data Structures",
            GitLabName = "data_structures"
        });

        await context.SaveChangesAsync();

        Assert.Equal(1, await storage.CountAsync());
    }
}
