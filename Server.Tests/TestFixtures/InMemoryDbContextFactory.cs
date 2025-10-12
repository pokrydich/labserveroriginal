namespace LabServer.Server.Tests.TestFixtures;

using LabServer.Server.Data;
using LabServer.Server.Service;

public static class InMemoryDbContextFactory
{
    public static LabsContext CreateContext(string databaseName, IGitLab gitLab)
    {
        var options = new DbContextOptionsBuilder<LabsContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        var context = new LabsContext(options, gitLab);
        context.Database.EnsureCreated();
        return context;
    }
}
