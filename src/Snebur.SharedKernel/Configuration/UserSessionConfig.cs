namespace Snebur.SharedKernel.Configuration;

public static class UserSessionConfig
{
    public const int UpdateCheckIntervalMinutes = 5;

    private static readonly TimeSpan PersistentSessionExpiration = TimeSpan.FromDays(90);
    private static readonly TimeSpan DefaultSessionExpiration = TimeSpan.FromMinutes(30);

    private static readonly TimeSpan PersistentSessionRefreshThreshold = TimeSpan.FromDays(7);
    private static readonly TimeSpan DefaultSessionRefreshThreshold = TimeSpan.FromMinutes(5);

    public static TimeSpan GetSessionExpiration(bool isPersistent)
    {
        return isPersistent
            ? PersistentSessionExpiration
            : DefaultSessionExpiration;
    }

    public static DateTime GetSessionExpirationDateUtc(bool isPersistent)
    {
        var expiration = GetSessionExpiration(isPersistent);
        return DateTime.UtcNow.Add(expiration);
    }

    public static bool NeedsRefreshSession(DateTime expiration, bool isPersistent)
    {
        var refreshThreshold = GetRefreshThreshold(isPersistent);
        return expiration.IsCloseTo(refreshThreshold);
    }

    private static TimeSpan GetRefreshThreshold(bool isPersistent)
    {
        return isPersistent
            ? PersistentSessionRefreshThreshold
            : DefaultSessionRefreshThreshold;
    }
}
