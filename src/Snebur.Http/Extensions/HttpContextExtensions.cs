using Snebur.Core.Utils;

namespace Snebur.Http.Extensions;

public static class HttpContextExtensions
{
    public static ClientRequestHeaderInfo GetRequestHeaderInfo(
        this HttpContext context)
    {
        Guard.NotNull(context);

        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        var userAgent = context.Request.Headers[HttpHeaderConstants.UserAgent].ToString() ?? "Unknown";
        var applicationName = context.Request.Headers[HttpHeaderConstants.ApplicationName].ToString() ?? "Unknown";
        var authorizationHeader = context.Request.Headers[HttpHeaderConstants.Authorization].ToString();
        var authorizationToken = JwtUtils.ExtractTokenFromAuthorizationHeader(authorizationHeader);

        return new ClientRequestHeaderInfo
        (
            IpAddress: ipAddress,
            UserAgent: userAgent,
            ApplicationName: applicationName,
            AuthorizationToken: authorizationToken
        );
    }
    public static string GetCookie(
        this HttpContext context,
        string cookieKey)
    {
        Guard.NotNull(context);

        if (!context.Request.Cookies.TryGetValue(cookieKey, out var cookieValue) ||
            string.IsNullOrWhiteSpace(cookieValue))
        {
            return default!;
        }
        return cookieValue;
    }

    public static Guid? TryGetCookieGuid(
      this HttpContext context,
      string cookieKey)
    {
        Guard.NotNull(context);

        if (context.Request.Cookies.TryGetValue(cookieKey, out var cookieValue) &&
            !string.IsNullOrWhiteSpace(cookieValue) &&
            Guid.TryParse(cookieValue, out var guidValue) && guidValue != Guid.Empty)
        {
            return guidValue;
        }
        return null;
    }

    public static void SetCookie(
        this HttpContext context,
        string cookieKey,
        string? cookieValue,
        TimeSpan? expirationTime = null)
    {
        Guard.NotNull(context);

        var options = new CookieOptions();
        if (expirationTime.HasValue)
        {
            options.Expires = DateTime.UtcNow.Add(expirationTime.Value);
        }
        context.Response.Cookies.Append(cookieKey, cookieValue ?? string.Empty, options);
    }
}

