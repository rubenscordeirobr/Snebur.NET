namespace Snebur.Core.Mappers;

public static class CultureMapper
{
    public static string MapCode(Culture culture)
    {
        return culture switch
        {
            // North America
            Culture.EnUs => "en-US",
            Culture.EnCa => "en-CA",
            Culture.EsMx => "es-MX",

            // South America
            Culture.EsAr => "es-AR",
            Culture.EsBo => "es-BO",
            Culture.PtBr => "pt-BR",
            Culture.EsCl => "es-CL",
            Culture.EsCo => "es-CO",
            Culture.EsEc => "es-EC",
            Culture.EnGy => "en-GY",
            Culture.EsPe => "es-PE",
            Culture.GnPy => "gn-PY",
            Culture.NlSr => "nl-SR",
            Culture.EsUy => "es-UY",
            Culture.EsVe => "es-VE",

            // Europe
            Culture.EsEs => "es-ES",
            Culture.DeDe => "de-DE",
            Culture.FrFr => "fr-FR",
            Culture.EnGb => "en-GB",
            Culture.ItIt => "it-IT",
            Culture.PtPt => "pt-PT",

            _ => throw new NotImplementedException($"Culture {culture} not implemented in CultureMapper")

        };
    }

    public static Culture? MapCulture(string? cultureCode)
    {
        return cultureCode?.ToLowerInvariant() switch
        {
            // North America
            "en-us" => Culture.EnUs,
            "en-ca" => Culture.EnCa,
            "es-mx" => Culture.EsMx,

            // South America
            "es-ar" => Culture.EsAr,
            "es-bo" => Culture.EsBo,
            "pt-br" => Culture.PtBr,
            "es-cl" => Culture.EsCl,
            "es-co" => Culture.EsCo,
            "es-ec" => Culture.EsEc,
            "en-gy" => Culture.EnGy,
            "es-pe" => Culture.EsPe,
            "gn-py" => Culture.GnPy,
            "nl-sr" => Culture.NlSr,
            "es-uy" => Culture.EsUy,
            "es-ve" => Culture.EsVe,

            // Europe
            "es-es" => Culture.EsEs,
            "de-de" => Culture.DeDe,
            "fr-fr" => Culture.FrFr,
            "en-gb" => Culture.EnGb,
            "it-it" => Culture.ItIt,
            "pt-pt" => Culture.PtPt,
            _ => null
        };
    }

    public static Country MapCountry(Culture culture)
    {
        return culture switch
        {
            // North America
            Culture.EnUs => Country.UnitedStates,
            Culture.EnCa => Country.Canada,
            Culture.EsMx => Country.Mexico,
            // South America
            Culture.EsAr => Country.Argentina,
            Culture.EsBo => Country.Bolivia,
            Culture.PtBr => Country.Brazil,
            Culture.EsCl => Country.Chile,
            Culture.EsCo => Country.Colombia,
            Culture.EsEc => Country.Ecuador,
            Culture.EnGy => Country.Guyana,
            Culture.EsPe => Country.Peru,
            Culture.GnPy => Country.Paraguay,
            Culture.NlSr => Country.Suriname,
            Culture.EsUy => Country.Uruguay,
            Culture.EsVe => Country.Venezuela,
            // Europe
            Culture.EsEs => Country.Spain,
            Culture.DeDe => Country.Germany,
            Culture.FrFr => Country.France,
            Culture.EnGb => Country.UnitedKingdom,
            Culture.ItIt => Country.Italy,
            Culture.PtPt => Country.Portugal,
            _ => throw new NotImplementedException($"Country for culture {culture} not implemented in CultureMapper")
        };
    }

    public static Currency MapCurrency(Culture culture)
    {
        return culture switch
        {
            // North America
            Culture.EnUs => Currency.USD,
            Culture.EnCa => Currency.USD, // Canada: CAD
            Culture.EsMx => Currency.USD,
          
            // South America
            Culture.EsAr => Currency.USD, // Argentina: ARS
            Culture.EsBo => Currency.USD, // Bolivia: BOB
            Culture.PtBr => Currency.BRL,
            Culture.EsCl => Currency.USD, // Chile: CLP
            Culture.EsCo => Currency.USD, // Colombia: COP
            Culture.EsEc => Currency.USD,
            Culture.EnGy => Currency.USD, // Guyana: GYD
            Culture.EsPe => Currency.USD, // Peru:PEN
            Culture.GnPy => Currency.USD, // Paraguay: PYG
            Culture.NlSr => Currency.USD, // Suriname: SRD
            Culture.EsUy => Currency.USD, // Uruguay: UYU
            Culture.EsVe => Currency.USD, // Venezuela: VES

            // Europe
            Culture.EsEs => Currency.EUR,
            Culture.DeDe => Currency.EUR,
            Culture.FrFr => Currency.EUR,
            Culture.EnGb => Currency.EUR, // UK: GBP 
            Culture.ItIt => Currency.EUR,
            Culture.PtPt => Currency.EUR,
            _ => throw new NotImplementedException($"Currency for culture {culture} not implemented in CultureMapper")
        };
    }

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

            _ => throw new NotImplementedException($"Language for culture {culture} not implemented in CultureMapper")
        };
    }

    public static Culture MapCultureFromCountry(Country country)
    {
        return country switch
        {
            // North America
            Country.UnitedStates => Culture.EnUs,
            Country.Canada => Culture.EnCa,
            Country.Mexico => Culture.EsMx,
            // South America
            Country.Argentina => Culture.EsAr,
            Country.Bolivia => Culture.EsBo,
            Country.Brazil => Culture.PtBr,
            Country.Chile => Culture.EsCl,
            Country.Colombia => Culture.EsCo,
            Country.Ecuador => Culture.EsEc,
            Country.Guyana => Culture.EnGy,
            Country.Peru => Culture.EsPe,
            Country.Paraguay => Culture.GnPy,
            Country.Suriname => Culture.NlSr,
            Country.Uruguay => Culture.EsUy,
            Country.Venezuela => Culture.EsVe,
            // Europe
            Country.Spain => Culture.EsEs,
            Country.Germany => Culture.DeDe,
            Country.France => Culture.FrFr,
            Country.UnitedKingdom => Culture.EnGb,
            Country.Italy => Culture.ItIt,
            Country.Portugal => Culture.PtPt,

            _ => throw new NotImplementedException($"Culture for country {country} not implemented in CultureMapper")
        };
    }
}
