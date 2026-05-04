using Microsoft.Extensions.Hosting;

namespace Snebur.Core.Extensions;

public static class HostEnvironmentExtensions
{
    public static bool IsDockerCompose(this IHostEnvironment hostEnvironment)
    {
        Guard.NotNull(hostEnvironment);

        if (!hostEnvironment.IsDevelopment())
        {
            return false;
        }

        var subEnv = Environment.GetEnvironmentVariable("ASPNETCORE_SUB_ENVIRONMENT");

        return string.Equals(
            subEnv,
            "DockerCompose",
            StringComparison.OrdinalIgnoreCase);

    }

    public static bool IsTest(this IHostEnvironment hostEnvironment)
    {
        Guard.NotNull(hostEnvironment);
        if (!hostEnvironment.IsDevelopment())
        {
            return false;
        }

        var subEnv = Environment.GetEnvironmentVariable("ASPNETCORE_SUB_ENVIRONMENT");
        return string.Equals(
            subEnv,
            "Test",
            StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsAspire(this IHostEnvironment hostEnvironment)
    {
        Guard.NotNull(hostEnvironment);
        if (!hostEnvironment.IsDevelopment())
        {
            return false;
        }

        var subEnv = Environment.GetEnvironmentVariable("ASPNETCORE_SUB_ENVIRONMENT");
        return string.Equals(
            subEnv,
            "Aspire",
            StringComparison.OrdinalIgnoreCase);

    }

    public static DirectoryInfo GetDevelopmentSrcDirectory(this IHostEnvironment hostEnvironment)
    {
        Guard.NotNull(hostEnvironment);

        if (!hostEnvironment.IsDevelopment())
        {
            throw new InvalidOperationException($"The method {nameof(GetDevelopmentSrcDirectory)} can only be used in development environment.");
        }

        return hostEnvironment.GetRequiredParentDirectory("src");
    }

    public static DirectoryInfo GetDevelopmentSrcOrTestsDirectory(this IHostEnvironment hostEnvironment)
    {
        Guard.NotNull(hostEnvironment);
        if (!hostEnvironment.IsDevelopment())
        {
            throw new InvalidOperationException($"The method {nameof(GetDevelopmentSrcDirectory)} can only be used in development environment.");
        }

        var parentFolder = EnvironmentHelper.IsXUnitTesting() ? "tests" : "src";
        return hostEnvironment.GetRequiredParentDirectory(parentFolder);
    }

    public static DirectoryInfo GetRequiredParentDirectory(this IHostEnvironment hostEnvironment, string folderName)
    {
        Guard.NotNull(hostEnvironment);
        Guard.NotNullOrWhiteSpace(folderName);

        var contentRoot = hostEnvironment.ContentRootPath;
        var directory = new DirectoryInfo(contentRoot);
        return directory.GetRequiredParent(folderName);
    }

    public static string GetDevelopmentSecretsPath(this IHostEnvironment hostEnvironment)
    {
        Guard.NotNull(hostEnvironment);

        if (!hostEnvironment.IsDevelopment())
        {
            throw new InvalidOperationException($"The method {nameof(GetDevelopmentSecretsPath)} can only be used in development environment.");
        }

        if (hostEnvironment.IsDockerCompose())
        {
            return "/opt/app/secrets";
        }

        var devDirectory = hostEnvironment.GetDevelopmentSrcOrTestsDirectory();
        return Path.GetFullPath(Path.Combine(devDirectory.FullName, "../.secrets"));
    }
}
