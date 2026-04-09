using Snebur.SharedKernel.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Snebur.SharedKernel.Localization;

public sealed class JsonStringLocalizerCache : IJsonStringLocalizerCache, IDisposable
{
    private readonly Dictionary<Language, LocalizationResourceMap> _localizedStringsCache = new();
    private readonly SemaphoreSlim _cacheLock = new(1, 1);

    private readonly IServiceProvider _serviceProvider;
    private readonly JsonLocalizationCacheConfiguration _configuration;
    private readonly ILogger<JsonStringLocalizerCache> _logger;

    public JsonStringLocalizerCache(
        IServiceProvider serviceProvider,
        JsonLocalizationCacheConfiguration configuration,
        ILogger<JsonStringLocalizerCache> logger)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task EnsureLanguageLoadedAsync(Language language)
    {
        if(language== Language.Default)
        {
            throw new ArgumentException("Language cannot be default.", nameof(language));
        }

        if (_localizedStringsCache.ContainsKey(language))
            return;

        try
        {
            await _cacheLock.WaitAsync();

          if (_localizedStringsCache.ContainsKey(language))
                return;

            await using var scope = _serviceProvider.CreateAsyncScope();
            var localizerService = scope.ServiceProvider.GetRequiredService<IJsonStringLocalizerService>();

            var result = await localizerService.GetLocalizationResourceMapAsync(language);
            if (result.IsFailure)
            {
                throw new InvalidOperationException(
                    $"Failed to initialize localization cache for language {language}: {result.Error}");
            }
            
            var resourceMap = result.Value;
            if (_configuration.AutoSeedMissingLocalization)
            {
                await SeedMissingLocalizationAsync(localizerService, resourceMap, language);
            }
            _localizedStringsCache[language] = resourceMap;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to load localization cache for language {Language}.", language);
            throw;
        }
        finally
        {
            _cacheLock.Release();
        }
    }

    public string GetLocalizedString(
        Language language,
        string resourceIdentifier,
        string localizationKey,
        string defaultValue)
    {
        if (language == Language.Default)
        {
            throw new ArgumentException("Language cannot be default.", nameof(language));
        }
        Guard.NotNullOrWhiteSpace(resourceIdentifier);

        CheckLanguageCacheLoaded(language);

        if (_localizedStringsCache.TryGetValue(language, out var resourceMap))
        {
            if (resourceMap.TryGetValue(resourceIdentifier, out var localizationMap))
            {
                if (localizationMap.TryGetValue(localizationKey, out var localizedString))
                {
                    if (language.IsSystemLanguage() && defaultValue != localizedString)
                    {
                        _ = UpdateDefaultLocalizedStringIfNeededAsync(resourceIdentifier, localizationKey, defaultValue);
                    }
                    return localizedString;
                }
            }
            _ = AddLocalizationStringIfNeededAsync(language, resourceIdentifier, localizationKey, defaultValue);
        }
        return defaultValue;
    }

    private void CheckLanguageCacheLoaded(Language language)
    {
        if (_localizedStringsCache.ContainsKey(language))
            return;

        throw new InvalidOperationException(
            $"Localization cache for language {language} is not initialized. " +
            $"Call LoadLanguageAsync first.");
    }

    private async Task AddLocalizationStringIfNeededAsync(
        Language language,
        string resourceIdentifier,
        string localizationKey,
        string defaultValue)
    {
        
        if (_configuration.AutoSeedMissingLocalization)
        {
            Guard.EnumDefined(language);
            Guard.NotNullOrWhiteSpace(resourceIdentifier);
            Guard.NotNullOrWhiteSpace(localizationKey);
            Guard.NotNullOrWhiteSpace(defaultValue);

            await using var scope = _serviceProvider.CreateAsyncScope();
            var localizerService = scope.ServiceProvider.GetRequiredService<IJsonStringLocalizerService>();

            await localizerService.AddLocalizedStringAsync(
                language,
                resourceIdentifier,
                localizationKey,
                defaultValue);
        }
    }

    private async Task UpdateDefaultLocalizedStringIfNeededAsync(
        string resourceKey,
        string localizationKey,
        string defaultValue)
    {
        if (_configuration.AutoUpdateDefaultKeys)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var localizerService = scope.ServiceProvider.GetRequiredService<IJsonStringLocalizerService>();

            await localizerService.UpdateDefaultLocalizedStringAsync(
                resourceKey,
                localizationKey,
                defaultValue);
        }
    }

    private async Task SeedMissingLocalizationAsync(
       IJsonStringLocalizerService localizerService,
       LocalizationResourceMap targetMap,
       Language language)
    {
        if (language.IsSystemLanguage())
        {
            return;
        }

        var seeder = new MissingLocalizationSeeder(localizerService, language);
        if (_localizedStringsCache.TryGetValue(LanguageHelper.SystemLanguage, out var sourceMap))
        {
            await seeder.SeedAsync(sourceMap, targetMap);
        }
        else
        {
            await seeder.SeedAsync(targetMap);
        }
    }

    public void Dispose()
    {
        try
        {
            _cacheLock.Dispose();
        }
        catch
        {
            // Ignore exceptions during disposal
        }
        GC.SuppressFinalize(this);
    }
}
