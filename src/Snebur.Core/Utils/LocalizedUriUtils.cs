using System.Diagnostics.CodeAnalysis;
using System.Web;
using Snebur.Core.Mappers;

namespace Snebur.Core.Utils;

public static class LocalizedUriUtils
{
    public const Culture DefaultCulture = Culture.EnUs;
    public static string DefaultCultureCode
        => CultureHelper.DefaultCultureCode;

    public static Culture GetCultureFromUri(Uri uri)
    {
        if (uri is null)
            return DefaultCulture;

        var cultureCode = ExtractCultureCodeFromUri(uri.AbsolutePath);
        if (cultureCode is not null)
        {
            return CultureHelper.GetCulture(cultureCode);
        }
        return DefaultCulture;
    }

    public static Culture GetCultureFromUri(string? uri)
    {
        if (string.IsNullOrWhiteSpace(uri))
        {
            return DefaultCulture;
        }

        return ExtractCultureUri(uri)
            ?? DefaultCulture;
    }

    public static string GetCultureCodeFromUri(string? uri)
    {
        if (uri is null)
            return DefaultCultureCode;

        return ExtractCultureCodeFromUri(uri)
            ?? DefaultCultureCode;
    }

    public static string? GetLanguageCodeFromQuery(string? path)
    {
        if (path is null)
            return null;

        var index = path.IndexOf('?');
        if (index == -1)
            return null;

        var query = path.Substring(index + 1);
        var queryParameters = HttpUtility.ParseQueryString(query);
        if (!queryParameters.HasKeys())
            return null;

        var langCode = queryParameters["lang"];
        if (string.IsNullOrEmpty(langCode))
            return null;
        
        return langCode.ToLowerInvariant();
    }

    public static Culture? ExtractCultureUri(string? uri)
    {
        if (uri is null)
            return null;

        var cultureCode = ExtractCultureCodeFromUri(uri);
        if (cultureCode is not null)
        {
            return CultureMapper.MapCulture(cultureCode);
        }
        return null;
    }

    public static string? ExtractCultureCodeFromUri(string? uri)
    {
        if (uri is null)
            return null;

        var path = UriUtils.IsAbsoluteUri(uri)
            ? new Uri(uri).AbsolutePath
            : uri;

        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length > 0 && IsCultureCodeSupported(segments[0]))
        {
            return segments[0].ToLowerInvariant();
        }

        return null;
    }

    public static string BuildDefaultCultureUri(string? uri)
    {
        var defaultCultureCode = CultureMapper.MapCode(DefaultCulture);
        return BuildLocalizedUri(uri, defaultCultureCode);
    }

    public static string BuildDefaultCultureUri(string? uri, string? queryString)
    {
        var uriWithCulture = BuildDefaultCultureUri(uri);
        if (string.IsNullOrWhiteSpace(queryString))
        {
            return uriWithCulture;
        }
        return $"{uriWithCulture}?{queryString}";
    }

    public static string BuildLocalizedUri(
        string? uri,
        string cultureCode)
    {
        Guard.NotEmpty(cultureCode);

        if (!IsCultureCodeSupported(cultureCode))
        {
            throw new ArgumentException($"Culture '{cultureCode}' is not supported.");
        }
        cultureCode = cultureCode.ToLowerInvariant();
        uri = uri?.Trim() ?? String.Empty;

        var currentCulture = ExtractCultureCodeFromUri(uri);
        if (currentCulture == cultureCode)
        {
            return uri;
        }

        if (!string.IsNullOrEmpty(currentCulture))
        {
            uri = RemoveLocalization(uri, currentCulture);
        }

        if (UriUtils.IsAbsoluteUri(uri))
        {
#pragma warning disable CS0618 
            var uriObj = new Uri(uri, dontEscape: true);
#pragma warning restore CS0618 

            var path = $"/{cultureCode}/{uriObj.PathAndQuery.Trim('/')}";
            return uriObj.IsDefaultPort
                ? $"{uriObj.Scheme}://{uriObj.Host}{path}"
                : $"{uriObj.Scheme}://{uriObj.Host}:{uriObj.Port}{path}";
        }

        return string.IsNullOrEmpty(uri)
                ? $"/{cultureCode}"
                : $"/{cultureCode}/{uri.Trim('/')}";

    }

    public static string RemoveLocalization(string uri)
    {
        var cultureCode = ExtractCultureCodeFromUri(uri);
        if (string.IsNullOrEmpty(cultureCode))
            return uri;

        return RemoveLocalization(uri, cultureCode);
    }

    private static string RemoveLocalization(string uri, string currentCulture)
    {
        if (string.IsNullOrEmpty(uri))
            return uri;

        if (UriUtils.IsAbsoluteUri(uri))
        {
            var uriObj = new Uri(uri);
            var path = uriObj.AbsolutePath.TrimStart('/');

            if (path.StartsWith(currentCulture, StringComparison.OrdinalIgnoreCase))
            {
                // Remove the culture segment
                var newPath = path.Substring(currentCulture.Length + 1);
                if (string.IsNullOrEmpty(newPath))
                    newPath = "/";
                else if (!newPath.StartsWith('/'))
                    newPath = $"/{newPath}";

                var builder = new UriBuilder(uriObj)
                {
                    Path = newPath
                };
                return builder.Uri.ToString();
            }
            return uri;
        }

        var segments = uri.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length > 0 && segments[0].Equals(currentCulture, StringComparison.OrdinalIgnoreCase))
        {
            var result = string.Join('/', segments[1..]);
            return string.IsNullOrEmpty(result) ? "/" : "/" + result;
        }
        return uri;
    }

    public static bool IsCultureCodeSupported(
      [NotNullWhen(true)]
        string? cultureCode)
    {
        if (cultureCode is null || cultureCode.Length < 2)
            return false;

        var culture = CultureMapper.MapCulture(cultureCode);
        return culture != null;
    }

    public static bool IsLocalizedUri(string? uri)
    {
        if (string.IsNullOrEmpty(uri))
            return false;

        return ExtractCultureCodeFromUri(uri) is not null;
    }
}
