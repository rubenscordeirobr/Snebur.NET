using Snebur.Application.Extensions;
using Snebur.Core.Extensions;
using Snebur.SharedKernel.Localization;
using Snebur.Testing.Core.Mocks;
using Microsoft.Extensions.Logging;

namespace Snebur.Testing.Core.Extensions;

public static class MockConfigurationExtensions
{
    public static IServiceCollection AddMockInfrastructureServices(
        this IServiceCollection services)
    {
        var localizationFolder = GetLocalizationFolder();
        var config = new JsonLocalizationConfiguration
        {
            ResourcesRootPath = localizationFolder,
            AutoSeedMissingLocalization = true,
            AutoUpdateDefaultKeys = true,
            AutoTranslate = true,
            CustomTranslationModelId = null
        };

        return services.AddSingleton<ISecureConfiguration, SecureConfigurationMock>()
            .AddSingleton<ICacheRepository, CacheRepositoryMock>()
            .AddSingleton<IEmailSender, EmailSenderMock>()
            .AddSingleton<IFileService, FileServiceMock>()
            .AddSingleton<JsonLocalizationCacheConfiguration>(config)
            .AddSingleton(config);
    }

    private static string GetLocalizationFolder()
    {
        var temp = AppContext.BaseDirectory;
        var current = new DirectoryInfo(temp);
        var testsDirectory = current.GetRequiredParent("tests");
        var localizationPath = Path.GetFullPath(Path.Combine(testsDirectory.FullName, "../lang"));
        Directory.CreateDirectory(localizationPath);
        return localizationPath;
    }

    public static IServiceCollection AddPersistenceServicesMock(
    this IServiceCollection services)
    {
        return services
            .AddSingleton<IActivityRepository, ActivityRepositoryMock>()
            .AddIdentityRepositoryServices();
    }

    public static IServiceCollection AddLoggerServiceMock(
        this IServiceCollection services)
    {
        return services.AddTransient(typeof(ILogger<>), typeof(TestOutputLogger<>));
    }

    public static IServiceCollection AddUserSessionAccessorMock<TRoleProvider>(
        this IServiceCollection services)
        where TRoleProvider : IRoleProvider, new()
    {
        services.AddTransient(provider =>
         {
             var accessor = provider.GetRequiredService<IHttpContextSessionAccessor>();
             return accessor.GetRequiredUserSession();
         });

        var roleProvider = new TRoleProvider();
        switch (roleProvider.UserRole)
        {
            case UserRole.Anonymous:
                return services.AddSingleton<IHttpContextSessionAccessor, AnonymousUserSessionAccessorMock>();

            case UserRole.Owner:
                return services.AddSingleton<IHttpContextSessionAccessor, TenantOwnerUserSessionAccessorMock>();

            case UserRole.Admin:
                return services.AddSingleton<IHttpContextSessionAccessor, AminUserSessionAccessorMock>();

            default:
                throw new NotImplementedException(
                    $"UserRole {roleProvider.UserRole} not implemented in MockConfigurationExtensions");
        }
    }
}
