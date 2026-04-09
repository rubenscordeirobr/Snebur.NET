using Snebur.SharedKernel.Localization;

namespace Snebur.SharedKernel.Abstractions;

public interface IJsonStringLocalizerService : ICommunicationService
{
    Task<Result<LocalizationResourceMap>> GetLocalizationResourceMapAsync(
        Language language, 
        CancellationToken cancellationToken = default);

    Task<Result<LocalizedStrings>> GetLocalizedStringsAsync(
        Language language,
        string resourceKey,
        CancellationToken cancellationToken = default);

    Task<Result<OperationResponse>> AddLocalizedStringAsync(
        Language language,
        string resourceKey,
        string localizationKey,
        string defaultValue,
        CancellationToken cancellationToken = default);

    Task<Result<OperationResponse>> UpdateDefaultLocalizedStringAsync(
        string resourceKey, 
        string localizationKey, 
        string defaultValue,
        CancellationToken cancellationToken = default);

}
