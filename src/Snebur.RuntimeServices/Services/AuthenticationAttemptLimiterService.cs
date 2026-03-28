using Snebur.Application.Abstractions.Security;
using Snebur.Application.Models.Security;
using Microsoft.Extensions.Logging;

namespace Snebur.RuntimeServices.Services;

public class AuthenticationAttemptLimiterService : CacheServiceBase, IAuthenticationAttemptLimiterService
{
    protected override string PrefixCacheName
       => "authentication-attempt-limiter";

    public AuthenticationAttemptLimiterService(
        ICacheRepository cacheRepository,
        ILogger<AuthenticationAttemptLimiterService> logger)
        : base(cacheRepository, logger, TimeSpan.FromHours(1))
    {
    }

    public async Task<MaxAuthenticationResult> MaxAuthenticationReachedAsync(
        string ipAddress,
        CancellationToken cancellationToken = default)
    {
        var record = await GetFromCacheAsync<AuthenticationAttemptRecord>(ipAddress, cancellationToken);
        if (record is not null && record.FailedAttempts >= 5)
        {
            var timeSinceLastAttempt = DateTime.UtcNow - record.LastFailedAttempt;
            if (timeSinceLastAttempt < TimeSpan.FromMinutes(record.FailedAttempts))
            {
                var expiration = TimeSpan.FromMinutes(record.FailedAttempts) - timeSinceLastAttempt;
                return new MaxAuthenticationResult(true, record.FailedAttempts, expiration);
            }
        }
        return MaxAuthenticationResult.Success;
    }

    public async Task IncrementFailedAttemptsAsync(
        string ipAddress,
        CancellationToken cancellationToken = default)
    {
        var record = await GetFromCacheAsync<AuthenticationAttemptRecord>(ipAddress, cancellationToken);
        if (record is null)
        {
            record = new AuthenticationAttemptRecord(1, DateTime.UtcNow);
        }
        else
        {
            record = record with
            {
                FailedAttempts = record.FailedAttempts + 1,
                LastFailedAttempt = DateTime.UtcNow
            };
        }
        await AddToCacheAsync(ipAddress, record);
    }

    private sealed record AuthenticationAttemptRecord(
        int FailedAttempts,
        DateTime LastFailedAttempt
    );
}

