using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Snebur.Application.Models.Secrets;
using Microsoft.Extensions.Logging;

namespace Snebur.RuntimeServices.Services.Azure;

public sealed class AzureTranslationService : ITranslationService, IDisposable
{
    private readonly AzureTranslationSecrets _azureSecrets;
    private readonly ILogger<AzureTranslationService> _logger;
    private readonly HttpClient _httpClient;

    public AzureTranslationService(
        AzureTranslationSecrets azureSecrets,
        ILogger<AzureTranslationService> logger)
    {
        _azureSecrets = azureSecrets;
        _logger = logger;
        _httpClient = new HttpClient();
    }
    public async Task<Result<string>> TextTranslateAsync(
        string textToTranslate,
        string fromLanguage,
        string toLanguage,
        string? customModelId = null)
    {
        try
        {
            return await TextTranslateAsyncInternal(
                textToTranslate,
                fromLanguage,
                toLanguage,
                customModelId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error translating text");

            var error = new AzureServiceError(
                ex,
                "AzureTranslationService.TranslationError",
                $"An error occurred while translating the text. Error: {ex.Message}");
            return Result.Failure<string>(error);

        }
    }
    private async Task<Result<string>> TextTranslateAsyncInternal(
        string textToTranslate,
        string fromLanguage,
        string toLanguage,
        string? customModelId = null)
    {
        var apiKey = _azureSecrets.Key;
        var endpoint = _azureSecrets.TextTranslationEndpoint;
        var location = _azureSecrets.Location;

        var route = $"/translate?api-version=3.0&from={fromLanguage}&to={toLanguage}";

        if (!string.IsNullOrWhiteSpace(customModelId))
        {
            //This will be implement late route += $"&customizationId={customModelId}";
            throw new NotImplementedException("Custom model ID is not implemented yet.");
        }

        var body = new object[] { new { Text = textToTranslate } };
        var requestBody = JsonSerializer.Serialize(body);

        using (var client = new HttpClient())
        using (var request = new HttpRequestMessage())
        {
            // Build the request.
            request.Method = HttpMethod.Post;
            request.RequestUri = new Uri(endpoint + route);
            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            request.Headers.Add("Ocp-Apim-Subscription-Key", apiKey);
            request.Headers.Add("Ocp-Apim-Subscription-Region", location);

            var response = await client.SendAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var translationResponses = await response.Content.ReadFromJsonAsync<List<TranslationResponse>>();
             
                if (!(translationResponses?.Count > 0 && translationResponses[0].Translations?.Count > 0))
                {
                    return Result.Failure<string>(
                        new AzureServiceError(
                            null,
                            "AzureTranslationService.TranslationError",
                            "No translation response received."));
                }

                var firstTranslation = translationResponses[0].Translations[0];
                if (firstTranslation == null)
                {
                    return Result.Failure<string>(
                        new AzureServiceError(
                            null,
                            "AzureTranslationService.TranslationError",
                            "No translation result found."));
                }
                return Result.Success(firstTranslation.Text);
            }

            _logger.LogError("Error in translation service: {StatusCode}", response.StatusCode);

            return Result.Failure<string>(
                new AzureServiceError(
                    null,
                    "AzureTranslationService.TranslationError",
                    $"An error occurred while translating the text. Status code: {response.StatusCode}"));
        }

    }

    public void Dispose()
    {
        _httpClient.Dispose();
        GC.SuppressFinalize(this);
    }
}

public class TranslationRequest
{
    public string Text { get; set; }
}

public class TranslationResponse
{
    public IReadOnlyList<TranslationResult> Translations { get; set; }
}

public class TranslationResult
{
    public string Text { get; set; }
    public string To { get; set; }
}
