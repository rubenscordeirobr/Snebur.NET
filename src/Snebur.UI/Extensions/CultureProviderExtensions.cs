using Snebur.Core.Helpers;
using Snebur.UI.Services;

namespace Snebur.UI.Extensions;

public static class CultureProviderExtensions
{
    public static async Task InitializeAsync(this ICultureProvider cultureProvider)
    {
        if (cultureProvider is not CultureProvider cultureProviderInstance)
        {
            throw new InvalidOperationException(
                $"The culture provider must be of type {nameof(CultureProvider)}.");
        }
        await cultureProviderInstance.InitializeAsync();
    }
    public static async Task SetCultureAndLanguageAsync(
        this ICultureProvider cultureProvider,
        string cultureCode, 
        string? languageCode)
    {
        if (cultureProvider is not CultureProvider cultureProviderInstance)
        {
            throw new InvalidOperationException(
                $"The culture provider must be of type {nameof(CultureProvider)}.");
        }
        await cultureProviderInstance.SetCultureAndLanguageAsync(cultureCode, languageCode);
    }

    public static Task SetCultureAsync(
        this ICultureProvider cultureProvider,
        string cultureCode)
    {
        if (cultureProvider is not CultureProvider concreteProvider)
        {
            throw new InvalidOperationException(
                $"The culture provider must be of type {nameof(CultureProvider)}.");
        }
        return concreteProvider.SetCultureCodeAsync(cultureCode);
    }

    public static async Task SetCountryAsync(
        this ICultureProvider cultureProvider,
        Country country)
    {
        if (cultureProvider is not CultureProvider concreteProvider)
        {
            throw new InvalidOperationException(
                $"The culture provider must be of type {nameof(CultureProvider)}.");
        }

        var culture = CultureHelper.GetCultureFromCountry(country);
        await concreteProvider.SetCultureAsync(culture);
    }

    public static async Task SetLanguageAsync(
        this ICultureProvider cultureProvider,
        Language language)
    {
        if (cultureProvider is not CultureProvider concreteProvider)
        {
            throw new InvalidOperationException(
                $"The culture provider must be of type {nameof(CultureProvider)}.");
        }

        await concreteProvider.SetLanguageAsync(language);
    }
}

