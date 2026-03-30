using System.Collections.Concurrent;
using System.Text.Json;

namespace Snebur.Testing.Core.Mocks.Infrastructure;

public class CacheRepositoryMock : ICacheRepository
{

    private readonly ConcurrentDictionary<string, string> _cache = new();

    protected virtual JsonSerializerOptions JsonSerializationOptions { get; }
        = new JsonSerializerOptions(JsonSerializerOptions.Web) { IncludeFields = true };

    public Task<bool> KeyExistsAsync(string cacheKey)
    {
        return Task.FromResult(_cache.ContainsKey(cacheKey));
    }

    public Task<string?> StringGetAsync(string cachedKey)
    {
        Guard.NotNull(cachedKey);
        return Task.FromResult(_cache.TryGetValue(cachedKey, out var value) ? value : null);
    }

    public Task KeyDeleteAsync(string cacheKey)
    {
        _cache.TryRemove(cacheKey, out _);
        return Task.CompletedTask;
    }

    public Task StringSetAsync(
        string cacheKey,
        string value,
        TimeSpan timeSpan)
    {
        _cache.AddOrUpdate(cacheKey, value, (_, _) => value);
        return Task.CompletedTask;
    }

    public void Clear(string keyPrefix)
    {
        var keys = _cache.Keys.Where(k => k.StartsWith(keyPrefix, StringComparison.Ordinal)).ToArray();
        foreach (var key in keys)
        {
            _cache.TryRemove(key, out _);
        }
    }
}

