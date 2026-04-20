
using Snebur.Core.Mappers;

namespace Snebur.Core.Helpers;

public static class LanguageHelper
{
    public static Language SystemLanguage => Language.English;

    public static Language? Normalize(Culture culture, Language? language)
    {
        if(language is null)
            return null;

        return Normalize(culture, language.Value);
    }

    public static Language Normalize(Culture culture, Language language)
    {
        return language == Language.Default
          ? CultureMapper.MapLanguage(culture)
          : language;
    }

    public static Language GetLanguage(string? languageCode)
    {
        var language = LanguageMapper.MapLanguage(languageCode);
        if (language.HasValue)
        {
            return language.Value;
        }

        var culture = CultureMapper.MapCulture(languageCode);
        if (culture.HasValue)
        {
            return CultureHelper.GetLanguage(culture.Value);
        }
        return SystemLanguage;
    }

    public static bool IsLanguageCodeSupported(string? languageCode)
    {
        if (languageCode is null || languageCode.Length < 2)
            return false;

        var language = LanguageMapper.MapLanguage(languageCode);
        return language != null;
    }
}
