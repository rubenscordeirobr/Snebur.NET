namespace Snebur.Core.Utils;

public static class JwtUtils
{
    public const string AuthenticationScheme = "Bearer";
    public static string? ExtractTokenFromAuthorizationHeader(string? authorizationHeader)
    {

        if (string.IsNullOrWhiteSpace(authorizationHeader))
        {
            return null;
        }

        if (IsBearerToken(authorizationHeader))
        {
            return authorizationHeader.Substring(AuthenticationScheme.Length).Trim();
        }
        return authorizationHeader;
    }

    public static bool IsBearerToken(string? authorizationHeader)
    {
        if (string.IsNullOrWhiteSpace(authorizationHeader))
        {
            return false;
        }
        return authorizationHeader.StartsWith(AuthenticationScheme, StringComparison.Ordinal);
    }

    public static string FormatAsAuthorizationHeader(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return string.Empty;
        }
        if (token.StartsWith(AuthenticationScheme, StringComparison.Ordinal))
        {
            return token;
        }
        return $"{AuthenticationScheme} {token}";
    }
}

