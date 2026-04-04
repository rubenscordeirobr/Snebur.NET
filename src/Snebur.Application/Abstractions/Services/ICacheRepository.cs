namespace Snebur.Application.Abstractions.Services;

public interface ICacheRepository : IApplicationService
{
    Task<bool> KeyExistsAsync(string cacheKey);
    Task<string?> StringGetAsync(string cachedKey);
    Task KeyDeleteAsync(string cacheKey);
    Task StringSetAsync(string cacheKey, string value, TimeSpan timeSpan);
}
