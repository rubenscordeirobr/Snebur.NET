using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace Snebur.FunctionalTests.Mocks;

public class IdentityWebHostMock<TRoleProvider>
    : WebApplicationFactory<Program>, IAsyncLifetime
    where TRoleProvider : IRoleProvider, new()
{
    public IdentityWebHostMock()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_SUB_ENVIRONMENT", "Test");
    }

    public void AddTestOutput(ITestOutputHelper output)
    {
        var serviceProvider = Services;
        var service = (TestOutputProxy)serviceProvider.GetRequiredService<ITestOutputHelper>()!;
        service.AddTestOutput(output);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureTestServices(services =>
        {
            services.AddLoggerServiceMock()
                .AddMockInfrastructureServices()
                .AddPersistenceServicesMock()
                .AddInMemoryIdentityDbContext();

            services.AddSingleton<ITestOutputHelper, TestOutputProxy>();
        });
    }

    #region IAsyncLifetime
    public ValueTask InitializeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
    }
     
    #endregion

}
