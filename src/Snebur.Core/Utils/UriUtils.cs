using System.Diagnostics.CodeAnalysis;

namespace Snebur.Core.Utils;

public static class UriUtils
{
    public static bool IsAbsoluteUri(
        [NotNullWhen(true)]
        string? uri)
    {
        if (uri is null)
            return false;

        uri= uri.Trim();
        return uri.StartsWith(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) ||
               uri.StartsWith(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase);
    }
}
