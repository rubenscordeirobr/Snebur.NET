using System.Diagnostics.CodeAnalysis;
using System.Web;
using Snebur.SharedKernel.Extensions;

namespace Snebur.UI.Services;

public class RouteService : IRouteService
{
    private readonly ICultureProvider _cultureProvider;

    public RouteService(ICultureProvider cultureProvider)
    {
        _cultureProvider = cultureProvider;
    }

    public string GetLocalizedUri(string? uri)
    {
        var localizedUri = GetLocalizedUriInternal(uri);
        if (_cultureProvider.IsLanguageMatchingCulture())
        {
            return localizedUri;
        }

        Guard.NotNull(localizedUri);

        var languageCode = _cultureProvider.GetLanguageCode();
        var parts = localizedUri.Split('?');
        var baseUri = parts[0];
        var queryString = parts.Length > 1 ? parts[1] : string.Empty;

        var queryParams = HttpUtility.ParseQueryString(queryString);
        queryParams["lang"] = languageCode;
        return $"{baseUri}?{queryParams}";
    }

    private string GetLocalizedUriInternal(string? uri)
    {
        var cultureCode = _cultureProvider.GetCultureCode();
        if (uri is null)
        {
            return $"/{cultureCode}";
        }

        var currentCulture = LocalizedUriUtils.ExtractCultureCodeFromUri(uri);
        if (cultureCode.Equals(currentCulture, StringComparison.OrdinalIgnoreCase))
        {
            return uri;
        }

        return LocalizedUriUtils.BuildLocalizedUri(uri, cultureCode!);
    }
}
