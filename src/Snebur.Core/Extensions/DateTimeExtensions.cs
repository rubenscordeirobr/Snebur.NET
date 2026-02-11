namespace Snebur.Core.Extensions;
public static class DateTimeExtensions
{
    public static bool IsExpired(
        this DateTime dateTime,
        TimeSpan expirationTime)
    {
        return dateTime.Add(expirationTime) < DateTime.UtcNow;
    }

    public static bool IsCloseTo(
        this DateTime dateTime,
        TimeSpan threshold)
    {
        return dateTime.Add(threshold) < DateTime.UtcNow;
    }
}
