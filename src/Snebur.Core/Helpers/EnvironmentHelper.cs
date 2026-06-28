namespace Snebur.Core.Helpers;

public static class EnvironmentHelper
{
    private static bool? _isDevelopment;
    private static bool? _isXUnitTest;

    public static bool IsDevelopment()
    {
        return _isDevelopment ??= Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
    }

    public static bool IsXUnitTesting()
    {
        return _isXUnitTest ??= Environment.GetEnvironmentVariable("XUNIT_ENVIRONMENT") == "TEST";
    }

    public static bool IsGithubActions()
    {
        return Environment.GetEnvironmentVariable("GITHUB_ACTIONS") == "true";
    }

    public static void Reset()
    {
        _isDevelopment = null;
        _isXUnitTest = null;

    }
}

