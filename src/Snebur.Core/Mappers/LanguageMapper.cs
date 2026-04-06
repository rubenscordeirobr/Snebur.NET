namespace Snebur.Core.Mappers;

public static class LanguageMapper
{
    public static Language MapLanguage(Culture culture)
    {
        return culture switch
        {
            // North America
            Culture.EnUs => Language.English,
            Culture.EnCa => Language.English,
            Culture.EsMx => Language.LatinSpanish,

            // South America
            Culture.EsAr => Language.LatinSpanish,
            Culture.EsBo => Language.LatinSpanish,
            Culture.PtBr => Language.PortugueseBrazil,
            Culture.EsCl => Language.LatinSpanish,
            Culture.EsCo => Language.LatinSpanish,
            Culture.EsEc => Language.LatinSpanish,
            Culture.EnGy => Language.English,
            Culture.EsPe => Language.LatinSpanish,
            Culture.GnPy => Language.LatinSpanish,
            Culture.NlSr => Language.LatinSpanish,
            Culture.EsUy => Language.LatinSpanish,
            Culture.EsVe => Language.LatinSpanish,

            // Europe
            Culture.EsEs => Language.Spanish,
            Culture.DeDe => Language.German,
            Culture.FrFr => Language.French,
            Culture.EnGb => Language.English,
            Culture.ItIt => Language.Italian,
            Culture.PtPt => Language.PortuguesePortugal,
            _ => throw new ArgumentOutOfRangeException(nameof(culture), culture, null)
        };
    }

    public static Language? MapLanguage(string? languageCode)
    {
        if (languageCode is null)
            return null;
        
        return languageCode.ToLowerInvariant() switch
        {
            "en" => Language.English,
            "fr" => Language.French,
            "de" => Language.German,
            "it" => Language.Italian,
            "pt-br" => Language.PortugueseBrazil,
            "pt-pt" => Language.PortuguesePortugal,
            "es-es" => Language.Spanish,
            "es-419" => Language.LatinSpanish,
            _ => null
        };
    }

    public static Culture MapCulture(Language language, Country country)
    {
        return (language, country) switch
        {
            //North America
            (Language.English, Country.UnitedStates) => Culture.EnUs,
            (Language.English, Country.Canada) => Culture.EnCa,
            (Language.LatinSpanish, Country.Mexico) => Culture.EsMx,
            (Language.French, Country.Canada) => Culture.FrCa,

            //South America
            (Language.PortugueseBrazil, _) => Culture.PtBr,
            (Language.LatinSpanish, Country.Argentina) => Culture.EsAr,
            (Language.LatinSpanish, Country.Bolivia) => Culture.EsBo,
            (Language.LatinSpanish, Country.Chile) => Culture.EsCl,
            (Language.LatinSpanish, Country.Colombia) => Culture.EsCo,
            (Language.LatinSpanish, Country.Ecuador) => Culture.EsEc,
            (Language.English, Country.Guyana) => Culture.EnGy,
            (Language.LatinSpanish, Country.Peru) => Culture.EsPe,
            (Language.LatinSpanish, Country.Paraguay) => Culture.GnPy,
            (Language.LatinSpanish, Country.Suriname) => Culture.NlSr,
            (Language.LatinSpanish, Country.Uruguay) => Culture.EsUy,
            (Language.LatinSpanish, Country.Venezuela) => Culture.EsVe,

            //Europe
            (Language.PortuguesePortugal, _) => Culture.PtPt,
            (Language.Spanish, _) => Culture.EsEs,
            (Language.English, Country.UnitedKingdom) => Culture.EnGb,
            (Language.French, _) => Culture.FrFr,
            (Language.German, _) => Culture.DeDe,
            (Language.Italian, _) => Culture.ItIt,
            _ => CultureHelper.DefaultCulture
        };
    }

    public static string MapCode(Language language)
    {
        return language switch
        {
            Language.Default => "en",
            Language.English => "en",
            Language.French => "fr",
            Language.German => "de",
            Language.Italian => "it",
            Language.PortugueseBrazil => "pt-BR",
            Language.PortuguesePortugal => "pt-PT",
            Language.LatinSpanish => "es-419",
            Language.Spanish => "es-ES",
            _ => throw new NotImplementedException($"MapToLanguageCode {language} not implemented")
        };
    }
}
