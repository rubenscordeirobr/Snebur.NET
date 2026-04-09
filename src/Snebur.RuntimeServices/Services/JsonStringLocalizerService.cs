using System.Text;
using Snebur.Core.Enums;
using Snebur.Core.Utils;
using Snebur.SharedKernel;
using Snebur.SharedKernel.Helpers;
using Snebur.SharedKernel.Localization;
using Microsoft.Extensions.Logging;

namespace Snebur.RuntimeServices.Services;

public sealed class JsonStringLocalizerService : IJsonStringLocalizerService, IDisposable
{
    private static readonly SemaphoreSlim _syncLock = new(1, 1);

    private readonly IFileService _fileService;
    private readonly ITranslationService _translationService;
    private readonly JsonLocalizationConfiguration _configuration;
    private readonly ILogger<JsonStringLocalizerService> _logger;

    public JsonStringLocalizerService(
        ITranslationService translationService,
        IFileService fileService,
        ILogger<JsonStringLocalizerService> logger,
        JsonLocalizationConfiguration configuration)
    {
        Guard.NotNull(configuration);

        _logger = logger;
        _fileService = fileService;
        _configuration = configuration;
        _translationService = translationService;
    }

    public async Task<Result<LocalizationResourceMap>> GetLocalizationResourceMapAsync(
        Language language,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _syncLock.WaitAsync(cancellationToken);

            var resourceMap = new LocalizationResourceMap();
            var culturePath = Path.Combine(_configuration.ResourcesRootPath, language.GetLanguageCode());
            var resourcesFiles = _fileService.GetFiles(
                culturePath,
                "*.json",
                SearchOption.AllDirectories,
                throwIfDirectoryDoesNotExist: false);

            foreach (var resourceFile in resourcesFiles)
            {
                var resourceId = LocalizationHelper.GetResourceKeyFromFileName(culturePath, resourceFile);
                var localizedStrings = await LoadLocalizedStringsAsync(resourceFile);
                resourceMap.Add(resourceId, localizedStrings);
            }
            return Result.Success(resourceMap);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load localized strings for culture {Culture}", language);
            var error = new UnknownError(ex, "JsonStringLocalizerService.GetLocalizedStringsAsync", $"Failed to load localized strings. Erro: {ex.Message} ");
            return Result.Failure<LocalizationResourceMap>(error);
        }
        finally
        {
            _syncLock.Release();
        }
    }

    public async Task<Result<LocalizedStrings>> GetLocalizedStringsAsync(
        Language language,
        string resourceKey,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _syncLock.WaitAsync(cancellationToken);
            var resourceFilePath = BuildResourceFilePath(language.GetLanguageCode(), resourceKey);
            var localizedStrings = await LoadLocalizedStringsAsync(resourceFilePath);
            return Result.Success(localizedStrings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to acquire lock for loading localized strings");

            var error = new UnknownError(ex, "JsonStringLocalizerService.GetLocalizedStringsAsync",
                $"Failed to acquire lock. Error: {ex.Message}");
            return Result.Failure<LocalizedStrings>(error);
        }
        finally
        {
            _syncLock.Release();
        }
    }

    public async Task<Result<OperationResponse>> AddLocalizedStringAsync(
        Language language,
        string resourceKey,
        string localizationKey,
        string defaultValue,
        CancellationToken cancellationToken = default)
    {
        if (!_configuration.AutoSeedMissingLocalization)
        {
            return Result.Success(new OperationResponse());
        }

        if (!language.IsSystemLanguage() && !_configuration.AutoTranslate)
        {
            return Result.Success(new OperationResponse());
        }

        var translatedResult = await GetTranslatedValueAsync(language, defaultValue);
        if (translatedResult.IsSuccess)
        {
            await AddOrUpdateLocalizedStringAsync(language, resourceKey, localizationKey, translatedResult.Value);
        }
        return Result.Success(new OperationResponse());
    }

    public async Task<Result<OperationResponse>> UpdateDefaultLocalizedStringAsync(
        string resourceKey,
        string localizationKey,
        string defaultValue,
        CancellationToken cancellationToken = default)
    {
        if (!_configuration.AutoUpdateDefaultKeys)
        {
            return Result.Success(new OperationResponse());
        }

        await AddOrUpdateLocalizedStringAsync(
            Language.Default,
            resourceKey,
            localizationKey,
            defaultValue);

        return Result.Success(new OperationResponse());
    }

    private async Task<LocalizedStrings> LoadLocalizedStringsAsync(string resourceFilePath)
    {
        if (!_fileService.FileExists(resourceFilePath))
        {
            return [];
        }

        try
        {
            var jsonContent = await _fileService.ReadAllTextAsync(resourceFilePath, Encoding.UTF8);
            return JsonUtils.Deserialize<LocalizedStrings>(jsonContent) ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deserialize JSON content from {ResourcePath}", resourceFilePath);
            return [];
        }
    }

    private async Task AddOrUpdateLocalizedStringAsync(
        Language language,
        string resourceKey,
        string localizationKey,
        string defaultValue)
    {
        var resourceRootPath = _configuration.ResourcesRootPath;
        if (!_fileService.DirectoryExists(resourceRootPath))
        {
            _logger.LogError("Resources root path does not exist: {ResourcesRootPath}", resourceRootPath);
            return;
        }

        try
        {
            await _syncLock.WaitAsync();

            var defaultCultureCode = language.GetLanguageCode();
            var resourceFilePath = BuildResourceFilePath(defaultCultureCode, resourceKey);
            var localizedStrings = await LoadLocalizedStringsAsync(resourceFilePath);
            if (localizedStrings.TryGetValue(localizationKey, out var currentTranslation))
            {
                if (currentTranslation == defaultValue)
                {
                    // No update required if the translation is already the default.
                    return;
                }
                localizedStrings[localizationKey] = defaultValue;
            }
            else
            {
                localizedStrings.Add(localizationKey, defaultValue);
            }
            await SaveLocalizedStringsAsync(resourceFilePath, localizedStrings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to acquire lock for adding/updating localized string");
        }
        finally
        {
            _syncLock.Release();
        }
    }

    private async Task SaveLocalizedStringsAsync(
        string resourceFilePath,
        Dictionary<string, string> localizedStrings)
    {
        var jsonContent = JsonUtils.Serialize(localizedStrings, JsonUtils.LocalizationJsonSerializerOptions);
        await _fileService.WriteAllTextAsync(resourceFilePath, jsonContent);
    }

    private string BuildResourceFilePath(string cultureCode, string resourceKey)
    {
        var resourceRootPath = _configuration.ResourcesRootPath;
        if (!_fileService.DirectoryExists(resourceRootPath))
        {
            _logger.LogError("Resources path does not exist: {ResourcesPath}", resourceRootPath);
            return string.Empty;
        }

        var recourseFileName = $"{resourceKey}.json";
        var resourcePath = Path.Combine(resourceRootPath, cultureCode, recourseFileName);
        return resourcePath;
    }
    private async Task<Result<string>> GetTranslatedValueAsync(Language language, string defaultValue)
    {
        if (language.IsSystemLanguage())
        {
            return Result.Success(defaultValue);
        }

        return await _translationService.TextTranslateAsync(
            defaultValue,
            "en",
            language.GetLanguageCode(),
            _configuration.CustomTranslationModelId);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}

