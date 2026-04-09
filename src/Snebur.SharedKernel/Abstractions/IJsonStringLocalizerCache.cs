namespace Snebur.SharedKernel.Abstractions;

public interface IJsonStringLocalizerCache
{
    string GetLocalizedString(
        Language language,
        string resourceIdentifier,
        string localizationKey,
        string defaultValue);

    Task EnsureLanguageLoadedAsync(Language language);
     
}
