using Snebur.Application;
using Snebur.RuntimeServices;
using Snebur.UseCases;
using Snebur.SharedKernel;
using Snebur.Testing.Core.Extensions;
using Snebur.SharedKernel.Extensions;
using Microsoft.Extensions.Configuration;

namespace Snebur.Testing.Core.Mocks;
public class ServiceProviderMock<TRoleProvider> : AbstractTestOutputServiceProvider
     where TRoleProvider : IRoleProvider, new()
{
    private readonly IServiceProvider _serviceProvider;

    protected UserRole UserRole { get; }

    protected override IServiceProvider ServiceProvider
        => _serviceProvider;
     

    public ServiceProviderMock()
    {
        _serviceProvider = BuildServiceProvider();
    }

    private IServiceProvider BuildServiceProvider()
    {
        var dic = new Dictionary<string, string>
        {
            ["LibreTranslate:TextTranslationEndpoint"] = "http://localhost:15000/"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(dic!)
            .Build() as IConfiguration;

        var serviceProvider = new ServiceCollection()
            .AddSharedKernelServices()
            .AddApplicationServices()
            .AddLoggerServiceMock()
            .AddRuntimeServices()
            .AddMockInfrastructureServices()
            .AddInMemoryIdentityDbContext()
            .AddUserSessionAccessorMock<TRoleProvider>()
            .AddPersistenceServicesMock()
            .AddUserCasesServices()
            .AddUserCasesSharedServices()
            .AddSingleton<ITestOutputHelper, TestOutputProxy>()
            .AddSingleton(configuration)
            .BuildServiceProvider();

        InitializeJsonLocalizerCache(serviceProvider);
        return serviceProvider;
            
    }

    private void InitializeJsonLocalizerCache(ServiceProvider serviceProvider)
    {
        var LocalizerCache = serviceProvider.GetRequiredService<IJsonStringLocalizerCache>();
        var task = LocalizerCache.EnsureSystemLanguageLoadedAsync();
        task.Wait();
    }
}
