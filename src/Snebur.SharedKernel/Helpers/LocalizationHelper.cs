using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Snebur.Core.Utils;

namespace Snebur.SharedKernel.Helpers;

public static class LocalizationHelper
{
    private const string SneburNamespace = "Snebur";
    private static readonly ConcurrentDictionary<Type, string> _cache = new();

    public static string GetResourceKey<T>()
    {
        return GetResourceKey(typeof(T));
    }

    internal static string GetResourceKey(Type type)
    {
        Guard.NotNull(type);

        if (_cache.TryGetValue(type, out var resourceKey))
        {
            return resourceKey;
        }

        resourceKey = GetResourceKeyInternal(type);
        _cache.TryAdd(type, resourceKey);
        return resourceKey;
    }

    private static string GetResourceKeyInternal(Type type)
    {
        var namespaceKey = GenerateKey(type.Namespace);
        var nameKey = GenerateKey(type.GetDisplayName(excludeNestedTypeNames: true));
        if (string.IsNullOrWhiteSpace(namespaceKey))
        {
            return nameKey;
        }

        return $"{namespaceKey}{Path.AltDirectorySeparatorChar}{nameKey.TrimStart(Path.AltDirectorySeparatorChar)}";
    }

    [return: NotNullIfNotNull(nameof(name))]
    private static string? GenerateKey(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        var fullName = name.Replace('.', Path.AltDirectorySeparatorChar)
            .Replace('+', '-')
            .Replace('<', '-')
            .Replace('>', '-');

        var parts = fullName.Split(Path.AltDirectorySeparatorChar);

        if (parts[0] == SneburNamespace)
        {
            parts[0] = string.Empty;
        }

        var sb = new StringBuilder();
        foreach (var part in parts)
        {
            if (!string.IsNullOrWhiteSpace(part))
            {
                if (sb.Length > 0)
                {
                    sb.Append(Path.AltDirectorySeparatorChar);
                }
                sb.Append(CaseConventionUtils.ToKebabCase(part));
            }
        }
        return sb.ToString();
    }

    public static string GetResourceKeyFromFileName(string culturePath, string resourceFile)
    {
        Guard.NotNullOrWhiteSpace(culturePath);
        Guard.NotNullOrWhiteSpace(resourceFile);

        var relativePath = Path.GetRelativePath(culturePath, resourceFile);
        var keyWithoutExtension = PathUtils.RemoveExtension(relativePath);
        var normalizedKey = keyWithoutExtension.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        return normalizedKey.TrimStart(Path.AltDirectorySeparatorChar);
    }
}
