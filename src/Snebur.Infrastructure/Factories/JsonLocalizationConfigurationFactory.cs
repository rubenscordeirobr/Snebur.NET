using Snebur.Infrastructure.Extensions;
using Snebur.SharedKernel.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Snebur.Infrastructure.Factories;

public static class JsonLocalizationConfigurationFactory
{
    public static JsonLocalizationConfiguration Create(
        IConfiguration configuration,
        IHostEnvironment hostEnvironment)
    {
        Guard.NotNull(configuration);
        Guard.NotNull(hostEnvironment);

        var isDevelopment = hostEnvironment.IsDevelopment();

        var autoAddMissingKeys = configuration.GetBoolean("Localization:AutoAddMissingKeys", isDevelopment);
        var autoUpdateDefaultKeys = configuration.GetBoolean("Localization:AutoUpdateDefaultKeys", isDevelopment);
        var autoTranslate = configuration.GetBoolean("Localization:AutoTranslate", isDevelopment);
        var customTranslationModelId = configuration["Localization:CustomTranslationModelId"] ;

        var resourcesRootPath = BuildResourcesRootPath(configuration, hostEnvironment);

        return new JsonLocalizationConfiguration
        {
            ResourcesRootPath = resourcesRootPath,
            AutoSeedMissingLocalization = autoAddMissingKeys ,
            AutoUpdateDefaultKeys = autoUpdateDefaultKeys ,
            AutoTranslate = autoTranslate ,
            CustomTranslationModelId = customTranslationModelId
        };
    }

    private static string BuildResourcesRootPath(IConfiguration configuration, IHostEnvironment hostEnvironment)
    {

        var configuredPath = configuration["Localization:ResourcesRootPath"];

        if (!string.IsNullOrEmpty(configuredPath) && Path.IsPathRooted(configuredPath))
        {
            return configuredPath;
        }

        if (string.IsNullOrEmpty(configuredPath))
        {
            if (hostEnvironment.IsDevelopment())
            {
                return GetDevelopmentLocalizationFolderPath(hostEnvironment);
            }

            throw new ArgumentException("Resources root path is not set in configuration and the application is not in development mode.");
        }

        var combinedPath = Path.Combine(hostEnvironment.ContentRootPath, configuredPath);
        var absolutePath = Path.GetFullPath(combinedPath);

        if (!Directory.Exists(absolutePath))
        {
            throw new DirectoryNotFoundException($"Resources root path does not exist: {absolutePath}");
        }
        return absolutePath;
    }

    private static string GetDevelopmentLocalizationFolderPath(IHostEnvironment hostEnvironment)
    {
        if (hostEnvironment.IsDockerCompose())
        {
            return "/opt/app/lang";
        }
         
        // Retrieve the 'src' folder path using the extension method.
        var srcDirectory = hostEnvironment.GetDevelopmentSrcOrTestsDirectory();
        // The Localization folder is assumed to be at the same level as the src folder.
        return Path.GetFullPath(Path.Combine(srcDirectory.FullName, "../lang"));
    }
}
