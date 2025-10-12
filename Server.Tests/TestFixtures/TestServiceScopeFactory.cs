namespace LabServer.Server.Tests.TestFixtures;

using LabServer.Server.Data;
using Microsoft.Extensions.DependencyInjection;

public sealed class TestServiceScopeFactory : IServiceScopeFactory, IDisposable
{
    private readonly ServiceProvider _serviceProvider;

    public TestServiceScopeFactory(LabsContext context)
    {
        var services = new ServiceCollection();
        services.AddScoped(_ => context);
        _serviceProvider = services.BuildServiceProvider();
    }

    public IServiceScope CreateScope() => _serviceProvider.CreateScope();

    public AsyncServiceScope CreateAsyncScope() => _serviceProvider.CreateAsyncScope();

    public void Dispose()
    {
        _serviceProvider.Dispose();
    }
}
