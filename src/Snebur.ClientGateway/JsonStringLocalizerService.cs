using System.Text.Json;
using Snebur.Core.Enums;
using Snebur.Core.Extensions;
using Snebur.Core.Utils;
using Snebur.SharedKernel.Abstractions;
using Snebur.SharedKernel.Localization;

namespace Snebur.ClientGateway;

[Route(RouteConstants.StringLocalizerService)]
[JsonOptionsProvider<LocalizationJsonOptionsProvider>]
public class JsonStringLocalizerService : IJsonStringLocalizerService
{
    private readonly IHttpClientMediator<JsonStringLocalizerService> _mediator;

    public JsonStringLocalizerService(IHttpClientMediator<JsonStringLocalizerService> mediator)
    {
        _mediator = mediator;
    }

    public Task<Result<LocalizationResourceMap>> GetLocalizationResourceMapAsync(
        Language language,
        CancellationToken cancellationToken = default)
    {
        var route = $"{language.GetLanguageCode().ToLowerInvariant()}";
        return _mediator.GetAsync<LocalizationResourceMap>(route, cancellationToken);
    }
     
    public Task<Result<LocalizedStrings>> GetLocalizedStringsAsync(
        Language language, 
        string resourceKey, 
        CancellationToken cancellationToken = default)
    {
        var route = $"{language.GetLanguageCode().ToLowerInvariant()}/{resourceKey}";
        return _mediator.GetAsync<LocalizedStrings>(route, cancellationToken);
    }

    public Task<Result<OperationResponse>> AddLocalizedStringAsync(
        Language language,
        string resourceKey,
        string localizationKey,
        string defaultValue,
        CancellationToken cancellationToken = default)
    {
        return _mediator.FormAsync<OperationResponse>(
            [nameof(language), nameof(resourceKey), nameof(localizationKey), nameof(defaultValue)],
            [language, resourceKey, localizationKey, defaultValue],
            cancellationToken);
    }

    public Task<Result<OperationResponse>> UpdateDefaultLocalizedStringAsync(
        string resourceKey,
        string localizationKey,
        string defaultValue,
        CancellationToken cancellationToken = default)
    {
        return _mediator.FormAsync<OperationResponse>(
            [nameof(resourceKey), nameof(localizationKey), nameof(defaultValue)],
            [resourceKey, localizationKey, defaultValue],
            cancellationToken);
    }
}

public class LocalizationJsonOptionsProvider : IJsonOptionsProvider
{
    public JsonSerializerOptions GetJsonOptions()
    {
        return JsonUtils.LocalizationJsonSerializerOptions;
    }
}
