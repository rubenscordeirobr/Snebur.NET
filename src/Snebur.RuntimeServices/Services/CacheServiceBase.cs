using System.Text.Json;
using Snebur.Core.Utils;
using Microsoft.Extensions.Logging;

namespace Snebur.RuntimeServices.Services;

public abstract class CacheServiceBase
{
    private readonly ICacheRepository _repository;
    private readonly ILogger _logger;
    private readonly TimeSpan _defaultExpiration;
    protected abstract string PrefixCacheName { get; }

    protected virtual JsonSerializerOptions JsonOptions { get; }
        = JsonUtils.CacheJsonSerializerOptions;
     
    protected CacheServiceBase(
        ICacheRepository _repository,
        ILogger logger,
        TimeSpan expiration)
    {
        this._repository = _repository;
        _logger = logger;
        _defaultExpiration = expiration;
    }

    protected async Task<bool> ExistsInCacheAsync(Guid key, CancellationToken cancellationToken)
    {
        var cacheKey = BuildCacheKey(key);
        return await _repository.KeyExistsAsync(cacheKey);
    }

    protected async Task<bool> ExistsInCacheAsync(string key, CancellationToken cancellationToken)
    {
        var cacheKey = BuildCacheKey(key);
        return await _repository.KeyExistsAsync(cacheKey);
    }

    protected async Task<T?> GetFromCacheAsync<T>(Guid key, CancellationToken cancellationToken)
    {
        var cacheKey = BuildCacheKey(key);
        return await GetAsyncInternal<T>(cacheKey, cancellationToken);
    }

    protected async Task<T?> GetFromCacheAsync<T>(string key, CancellationToken cancellationToken)
    {
        var cacheKey = BuildCacheKey(key);
        return await GetAsyncInternal<T>(cacheKey, cancellationToken);
    }

    protected async Task AddToCacheAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        var cacheKey = BuildCacheKey(key);
        await AddToCacheAsyncInternal(cacheKey, value, expiration);
    }

    protected async Task AddToCacheAsync<T>(Guid key, T value, TimeSpan? expiration = null)
    {
        var cacheKey = BuildCacheKey(key);
        await AddToCacheAsyncInternal(cacheKey, value, expiration);
    }

    protected async Task RemoveFromCacheAsync(Guid key)
    {
        var cacheKey = BuildCacheKey(key);
        await RemoveFromCacheAsyncInternal(cacheKey);
    }

    protected async Task RemoveFromCacheAsync(string key)
    {
        var cacheKey = BuildCacheKey(key);
        await RemoveFromCacheAsyncInternal(cacheKey);
    }

    private async Task<T?> GetAsyncInternal<T>(string cachedKey, CancellationToken cancellationToken = default)
    {
        var cachedValue = await _repository.StringGetAsync(cachedKey);
        if (cachedValue is null || cancellationToken.IsCancellationRequested)
        {
            return default;
        }

        try
        {
            var options = GetJsonOptions<T>();
            return JsonUtils.Deserialize<T>(cachedValue, options);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize cached value for key {Key} to {Type}", cachedKey, typeof(T).Name);
            return default;
        }
    }

    private async Task AddToCacheAsyncInternal<T>(string cacheKey, T value, TimeSpan? expiration = null)
    {
        JsonUtils.EnableIndentationInDevelopment(JsonOptions);
        var serializedValue = JsonUtils.Serialize(value, options: JsonOptions);
        await _repository.StringSetAsync(cacheKey, serializedValue, expiration ?? _defaultExpiration);
    }

    private async Task RemoveFromCacheAsyncInternal(string cacheKey)
    {
        await _repository.KeyDeleteAsync(cacheKey);
    }

    private string BuildCacheKey(
         string id)
         => BuildCacheKey(HashHelper.CreateMd5GuidHash(id));

    private string BuildCacheKey(Guid id)
        => $"{PrefixCacheName}:{id}";

    private JsonSerializerOptions? GetJsonOptions<T>()
    {
        if (typeof(T).IsSubclassOf(typeof(EntityBase)))
        {
            throw new InvalidOperationException(
                $"Caching is not supported for entity type   {typeof(T).Name} is a Entity." +
                $"Create a corresponding  Cached{typeof(T).Name} class for Entity representation.");
        }
        return JsonOptions;
    }
}
