using Snebur.Core.Helpers;
using Snebur.Core.Mappers;

namespace Snebur.UI.Services;

public class CultureProvider : ICultureProvider
{
    private Language? _language;
    private Culture _culture = CultureHelper.DefaultCulture;

    private readonly IJsonStringLocalizerCache _stringLocalizerCache;

    public CultureProvider(IJsonStringLocalizerCache stringLocalizerCache)
    {
        _stringLocalizerCache = stringLocalizerCache;
    }

    public Culture Culture
        => _culture;

    public Language Language
        => _language ?? CultureHelper.GetLanguage(_culture);

    public async Task InitializeAsync()
    {
        await _stringLocalizerCache.EnsureLanguageLoadedAsync(Language);
    }

    public async Task SetCultureAndLanguageAsync(string cultureCode, string? languageCode)
    {
        await SetCultureCodeAsync(cultureCode);
        await SetLanguageCodeAsync(languageCode);
    }

    public async Task SetCultureCodeAsync(string cultureCode)
    {
        var culture = CultureHelper.GetCulture(cultureCode);
        await SetCultureAsync(culture);
    }

    public async Task SetCultureAsync(Culture culture)
    {
        if (_culture == culture)
            return;

        var language = CultureHelper.GetLanguage(culture);
        await _stringLocalizerCache.EnsureLanguageLoadedAsync(language);
        _culture = culture;
    }

    public async Task SetLanguageCodeAsync(string? languageCode)
    {
        var language = LanguageMapper.MapLanguage(languageCode);
        await SetLanguageAsync(language);
    }

    public async Task SetLanguageAsync(Language? language)
    {
        var normalizedLanguage = LanguageHelper.Normalize(Culture, language);
        if(_language == normalizedLanguage)
            return;

        if (normalizedLanguage.HasValue)
        {
            await _stringLocalizerCache.EnsureLanguageLoadedAsync(normalizedLanguage.Value);
        }
        _language = normalizedLanguage;
    }
}
