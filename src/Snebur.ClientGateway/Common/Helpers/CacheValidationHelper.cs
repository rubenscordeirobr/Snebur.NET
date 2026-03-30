
using System.Text;

namespace Snebur.ClientGateway.Common.Helpers;

public static class CacheValidationHelper
{
    internal static string CreateCacheKey(
        string route,
        IReadOnlyList<KeyValuePair<string, string>> keyValuePairs)
    {
        Guard.NotNull(route);
         
        if (keyValuePairs == null || keyValuePairs.Count == 0)
        {
            return route;
        }

        var sortedPairs = keyValuePairs
            .OrderBy(kvp => kvp.Key, StringComparer.Ordinal)
            .ToList();

        var builder = new StringBuilder();
        builder.Append(route);
        builder.Append(':');

        foreach (var kvp in sortedPairs)
        {
            builder.Append(kvp.Key);
            builder.Append('=');
            builder.Append(kvp.Value);
            builder.Append(';');
        }

        return builder.ToString();
    }
}

