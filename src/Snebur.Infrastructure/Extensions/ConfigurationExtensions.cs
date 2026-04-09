using Microsoft.Extensions.Configuration;

namespace Snebur.Infrastructure.Extensions;

public static class ConfigurationExtensions
{
    public static bool GetBoolean(this IConfiguration configuration, string key, bool defaultValue)
    {
        Guard.NotNull(configuration);

        var value = configuration[key];
        if (string.IsNullOrEmpty(value))
        {
            return defaultValue;
        }

        return bool.TryParse(value, out var result) ? result : defaultValue;
    }
}
