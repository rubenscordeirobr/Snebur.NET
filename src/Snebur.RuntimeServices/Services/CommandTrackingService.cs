using Microsoft.Extensions.Logging;

namespace Snebur.RuntimeServices.Services;

public class CommandTrackingService : CacheServiceBase, ICommandTrackingService
{
    protected override string PrefixCacheName 
        => "request-tracker";

    public CommandTrackingService(
        ICacheRepository cacheRepository,
        ILogger<CommandTrackingService> logger)
        : base(cacheRepository, logger, TimeSpan.FromMinutes(5))
    {
    }

    public async Task<bool> ExistsAsync(Guid clientRequestId, CancellationToken cancellationToken = default)
    {
        return await ExistsInCacheAsync(clientRequestId, cancellationToken);
    }

    public async Task<Result<T>?> TryGetResultAsync<T>(
        Guid clientRequestId, 
        CancellationToken cancellationToken = default)
        where T : notnull
    {
        var value = await GetFromCacheAsync<T>(clientRequestId, cancellationToken);
        if (value is null)
        {
            return null;
        }
        return Result.Success(value);
    }

    public async Task TrackAsync<T>(Guid clientRequestId, Result<T> result) where T : notnull
    {
        Guard.NotNull(result);

        if (!result.IsSuccess)
        {
            throw new InvalidOperationException("Only successful results can be tracked.");
        }
        await AddToCacheAsync(clientRequestId, result.Value);
    }
}
