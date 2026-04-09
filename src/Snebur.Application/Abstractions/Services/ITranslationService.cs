namespace Snebur.Application.Abstractions.Services;

public interface ITranslationService
{
    Task<Result<string>> TextTranslateAsync(
        string textToTranslate,
        string fromLanguage,
        string toLanguage,
        string? customModelId = null);
}
