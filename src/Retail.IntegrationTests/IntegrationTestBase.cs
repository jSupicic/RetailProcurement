using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Retail.IntegrationTests;

public abstract class IntegrationTestBase : IClassFixture<IntegrationTestWebAppFactory>, IDisposable
{
    protected readonly IServiceScope Scope;
    protected IServiceProvider Services => Scope.ServiceProvider;

    public IntegrationTestBase(IntegrationTestWebAppFactory factory)
    {
        Scope = factory.Services.CreateScope();
    }

    protected T GetService<T>() => Services.GetRequiredService<T>();

    public void Dispose()
    {
        Scope.Dispose();
    }
}