using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Snebur.Infrastructure.Cache;

public class CacheRepository : ICacheRepository
{
    private readonly IDatabase _database;
    private readonly ILogger<CacheRepository> _logger;

    public CacheRepository(
        IConnectionMultiplexer connection,
        ILogger<CacheRepository> logger)
    {
        Guard.NotNull(connection);

        _database = connection.GetDatabase();
        _logger = logger;
    }

    public async Task<bool> KeyExistsAsync(string cacheKey)
    {
        try
        {
            return await _database.KeyExistsAsync(cacheKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if key exists in cache");
            return false;
        }
    }

    public async Task<string?> StringGetAsync(string cachedKey)
    {
        try
        {
            var cachedValue = await _database.StringGetAsync(cachedKey);
            if (!cachedValue.HasValue)
            {
                return null;
            }
            return cachedValue.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting value from cache");
            return null;
        }
    }

    public async Task KeyDeleteAsync(string cacheKey)
    {
        try
        {
            await _database.KeyDeleteAsync(cacheKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting key from cache");
        }
    }

    public async Task StringSetAsync(string cacheKey, string value, TimeSpan timeSpan)
    {
        try
        {
            await _database.StringSetAsync(cacheKey, value, timeSpan);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting value in cache");
        }
    }
}
