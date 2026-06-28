using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Snebur.Core.Utils;

namespace Snebur.RuntimeServices.Services;

public sealed class LibreTranslationService : ITranslationService, IDisposable
{
    private readonly ILogger<LibreTranslationService> _logger;
    private readonly HttpClient _httpClient;

    private readonly string _endpoint;
    public LibreTranslationService(
        ILogger<LibreTranslationService> logger,
        IConfiguration configuration)
    {
        Guard.NotNull(configuration);

        _logger = logger;
        _httpClient = new HttpClient();
        _endpoint = configuration["LibreTranslate:TextTranslationEndpoint"]?.TrimEnd('/')
            ?? string.Empty;
    }

    public async Task<Result<string>> TextTranslateAsync(
        string textToTranslate,
        string fromLanguage,
        string toLanguage,
        string? customModelId = null)
    {
        try
        {
            return await TextTranslateAsyncInternal(textToTranslate, fromLanguage, toLanguage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error translating text via LibreTranslate");

            var error = new TranslationServiceError(
                ex,
                "LibreTranslateService.TranslationError",
                $"An error occurred while translating the text. Error: {ex.Message}");

            return Result.Failure<string>(error);
        }
    }

    private async Task<Result<string>> TextTranslateAsyncInternal(
        string textToTranslate,
        string fromLanguage,
        string toLanguage)
    {

        if (string.IsNullOrWhiteSpace(_endpoint))
        {
            return Result.Failure<string>(
                new TranslationServiceError(
                    null,
                    "LibreTranslateService.ConfigurationError",
                    "TextTranslationEndpoint is not configured."));
        }

        var url = _endpoint + "/translate";

        var request = new LibreTranslateRequest
        {
            q = textToTranslate,
            source = fromLanguage,
            target = toLanguage,
            format = "text",
            api_key = string.Empty
        };

        using var content = JsonContent.Create(request);

        var response = await _httpClient.PostAsync(url, content).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError(
                "LibreTranslate responded with status code {StatusCode}",
                response.StatusCode);

            return Result.Failure<string>(
                new TranslationServiceError(
                    null,
                    "LibreTranslateService.TranslationError",
                    $"Status code: {response.StatusCode}"));
        }

        var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        try
        {
            var parsed = JsonUtils.Deserialize<LibreTranslateResponse>(body);
            if (parsed == null || string.IsNullOrWhiteSpace(parsed.translatedText))
            {
                return Result.Failure<string>(
                    new TranslationServiceError(
                        null,
                        "LibreTranslateService.TranslationError",
                        "No translation returned."));
            }

            return Result.Success(parsed.translatedText);
        }
        catch (JsonException ex)
        {
            _logger.LogError(
                ex,
                "Failed to parse LibreTranslate response: {Body}",
                body);

            return Result.Failure<string>(
                new TranslationServiceError(
                    ex,
                    "LibreTranslateService.TranslationError",
                    "Failed to parse translation response."));
        }
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        GC.SuppressFinalize(this);
    }

    private class LibreTranslateRequest
    {
        public string q { get; set; }
        public string source { get; set; }
        public string target { get; set; }
        public string format { get; set; }
        public string api_key { get; set; }
    }

    private class LibreTranslateResponse
    {
        [JsonPropertyName("translatedText")]
        public string translatedText { get; set; }
    }
}
