using Snebur.SharedKernel.Abstractions;

namespace Snebur.SharedKernel.Extensions;

public static class JsonStringLocalizerCacheExtensions
{
    public static Task EnsureSystemLanguageLoadedAsync(
        this IJsonStringLocalizerCache localizerCache)
    {
        Guard.NotNull(localizerCache);
        return localizerCache.EnsureLanguageLoadedAsync(LanguageHelper.SystemLanguage);
    }
    public static Task EnsureLanguageLoadedAsync(
        this IJsonStringLocalizerCache localizerCache,
        Culture culture)
    {
        Guard.NotNull(localizerCache);

        var language = CultureHelper.GetLanguage(culture);
        return localizerCache.EnsureLanguageLoadedAsync(language);
    }
}
