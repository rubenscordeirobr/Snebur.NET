using Snebur.Core.Mappers;
using Snebur.SharedKernel.Abstractions;

namespace Snebur.SharedKernel.Extensions;

public static class CultureProviderExtensions
{
    public static string GetCultureCode(this ICultureProvider cultureProvider)
    {
        Guard.NotNull(cultureProvider);

        return CultureHelper.GetCultureCode(cultureProvider.Culture);
    }

    public static string GetLanguageCode(this ICultureProvider cultureProvider)
    {
        Guard.NotNull(cultureProvider);
        return LanguageMapper.MapCode(cultureProvider.Language);
    }

    public static Country GetCountry(this ICultureProvider cultureProvider)
    {
        Guard.NotNull(cultureProvider);

        return CultureHelper.GetCountry(cultureProvider.Culture);
    }

    public static Currency GetCurrency(this ICultureProvider cultureProvider)
    {
        Guard.NotNull(cultureProvider);

        return CultureHelper.GetCurrency(cultureProvider.Culture);
    }

    public static bool IsLanguageMatchingCulture(
        this ICultureProvider cultureProvider)
    {
        Guard.NotNull(cultureProvider);

        var culture = cultureProvider.Culture;
        var sessionLanguage = cultureProvider.Language;
        var cultureLanguage = CultureHelper.GetLanguage(culture);
        return sessionLanguage == cultureLanguage;
    }
}
