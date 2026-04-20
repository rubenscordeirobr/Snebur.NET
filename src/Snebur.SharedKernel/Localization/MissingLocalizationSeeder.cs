using Snebur.SharedKernel.Abstractions;

namespace Snebur.SharedKernel.Localization;

internal class MissingLocalizationSeeder
{
    private readonly IJsonStringLocalizerService _localizerService;
    private readonly Language _targetLanguage;

    public MissingLocalizationSeeder(
        IJsonStringLocalizerService localizerService,
        Language targetLangauge)
    {
        if (targetLangauge == Language.Default)
        {
            throw new ArgumentException(
                "Target language cannot be default.", nameof(targetLangauge));
        }

        _localizerService = localizerService;
        _targetLanguage = targetLangauge;
    }

    public async Task<bool> SeedAsync(LocalizationResourceMap targetMap, CancellationToken cancellationToken = default)
    {
        if (_targetLanguage == LanguageHelper.SystemLanguage)
        {
            return false;
        }

        var sourceMap = await LoadSystemLanguageResourceMapAsync(cancellationToken);
        return await SeedAsync(sourceMap, targetMap, cancellationToken);
    }

    public async Task<bool> SeedAsync(
        LocalizationResourceMap sourceMap,
        LocalizationResourceMap targetMap,
        CancellationToken cancellationToken = default)
    {
        if (_targetLanguage == LanguageHelper.SystemLanguage)
        {
            return false;
        }

        var anyResourceChanged = false;

        foreach (var resourceId in sourceMap.Keys)
        {
            var defaultStrings = sourceMap[resourceId];
            var targetStrings = targetMap.GetValueOrDefault(resourceId) ?? new LocalizedStrings();

            var resourceChanged = await SyncMissingLocalizedStringsAsync(
                resourceId,
                defaultStrings,
                targetStrings,
                cancellationToken);

            if (resourceChanged)
            {
                anyResourceChanged = true;
            }
        }
        return anyResourceChanged;
    }

    private async Task<LocalizationResourceMap> LoadSystemLanguageResourceMapAsync(CancellationToken cancellationToken = default)
    {
        var result = await _localizerService.GetLocalizationResourceMapAsync(
           LanguageHelper.SystemLanguage,
           cancellationToken);

        if (result.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to load system language localization resource map. " +
                $"Error: {result.Error.Message}");
        }
        return result.Value;
    }

    private async Task<bool> SyncMissingLocalizedStringsAsync(
        string resourceKey,
        LocalizedStrings defaultStrings,
        LocalizedStrings targetStrings,
        CancellationToken cancellationToken = default)
    {
        var hasChanges = false;

        foreach (var key in defaultStrings.Keys)
        {
            if (!targetStrings.ContainsKey(key))
            {
                hasChanges = true;

                var defaultValue = defaultStrings[key];
                await _localizerService.AddLocalizedStringAsync(
                    _targetLanguage,
                    resourceKey,
                    key,
                    defaultValue,
                    cancellationToken);
            }
        }
        return hasChanges;
    }
}

