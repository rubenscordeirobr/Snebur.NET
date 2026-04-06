using Snebur.Core.Mappers;

namespace Snebur.Core.Extensions;

public static class LanguageExtensions
{
    public static bool IsSystemLanguage(this Language language)
    {
        return language ==  LanguageHelper.SystemLanguage;
    }

    public static string GetLanguageCode(this Language language)
    {
        return LanguageMapper.MapCode(language);
    }
}
