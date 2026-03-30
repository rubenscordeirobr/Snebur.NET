using Snebur.Core.Utils;

namespace Snebur.Infrastructure.Helpers;

internal static class SecureConfigHelper
{
    public static string BuildConfigurationKey(string key)
       => $"AppSettings:{key}";

    public static string BuildEnvironmentKey(string key)
        => "snebur_" + CaseConventionUtils.ToScreamingSnakeCase(key);
}

