using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Snebur.ClientGateway.Common;

public class ValidationResultCache
{
    private const int CacheExpirationSeconds = 30;
    private readonly ConcurrentDictionary<string, ValidationResultEntry> cache = new();

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out bool value)
    {

        if (cache.TryGetValue(key, out var entry))
        {
            if ((DateTime.UtcNow - entry.TimeAdded) < TimeSpan.FromSeconds(CacheExpirationSeconds))
            {
                value = entry.Value;
                return true;
            }
            else
            {
                cache.TryRemove(key, out _);
            }
        }
        value = default;
        return false;

    }

    public void Add(string key, bool value)
    {
        lock (cache)
        {
            cache[key] = new ValidationResultEntry(value, DateTime.UtcNow);
        }
    }
}
public record ValidationResultEntry(bool Value, DateTime TimeAdded);
