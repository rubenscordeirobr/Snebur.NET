using Snebur.SharedKernel.Models.Identities;
using Microsoft.Extensions.Logging;

namespace Snebur.RuntimeServices.Services;

public class UserSessionCacheService : CacheServiceBase, IUserSessionCacheService
{
    protected override string PrefixCacheName => "user-session";

    public UserSessionCacheService(
        ICacheRepository cacheRepository,
        ILogger<UserSessionCacheService> logger)
        : base(cacheRepository, logger, TimeSpan.FromHours(1))
    {
    }

    public Task<bool> ExistsAsync(Guid session_Id,
        CancellationToken cancellationToken = default)
    {
        return ExistsInCacheAsync(session_Id, cancellationToken);
    }

    public async Task<CachedUserSession?> GetSessionAsync(
        Guid session_Id,
        CancellationToken cancellationToken = default)
    {
        return await GetFromCacheAsync<CachedUserSession>(session_Id, cancellationToken);
    }

    public async Task AddSessionAsync(
        IUserSession session,
        CancellationToken cancellationToken = default)
    {
        Guard.NotNull(session);

        if (session.Id == Guid.Empty)
        {
            throw new InvalidOperationException("Cannot add session with empty Id");
        }

        var cachedSession = CachedUserSession.Create(session);
        await AddToCacheAsync(session.Id, cachedSession);
    }

    public Task RemoveSessionAsync(
        Guid session_Id,
        CancellationToken cancellationToken = default)
    {
        return RemoveFromCacheAsync(session_Id);
    }
}
