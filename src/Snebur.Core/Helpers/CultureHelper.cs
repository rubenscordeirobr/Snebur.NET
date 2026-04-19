using Snebur.Core.Mappers;
using Snebur.Core.Utils;

namespace Snebur.Core.Helpers;

public static class CultureHelper
{
    public const Culture DefaultCulture = Culture.EnUs;
    public static string DefaultCultureCode
        => CultureMapper.MapCode(DefaultCulture);

    public static Culture GetCulture(string? cultureCode)
    {
        if (EnumUtils.TryParse<Culture>(cultureCode, out var culture))
        {
            return culture;
        }
        return CultureMapper.MapCulture(cultureCode) ?? DefaultCulture;
    }

    public static Culture GetCultureFromCountry(Country country)
    {
        return CultureMapper.MapCultureFromCountry(country);
    }

    public static string GetCultureCode(Culture culture)
    {
        return CultureMapper.MapCode(culture);
    }

    public static Country GetCountry(Culture culture)
    {
        return CultureMapper.MapCountry(culture);
    }

    public static Currency GetCurrency(Culture culture)
    {
        return CultureMapper.MapCurrency(culture);
    }

    public static Language GetLanguage(Culture culture)
    {
        return CultureMapper.MapLanguage(culture);
    }

  
}

