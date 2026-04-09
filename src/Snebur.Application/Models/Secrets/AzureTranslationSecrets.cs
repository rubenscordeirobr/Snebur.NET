namespace Snebur.Application.Models.Secrets;

public record AzureTranslationSecrets
{
    public string Key { get; init; }
    public string TextTranslationEndpoint { get; init; }
    public string DocumentTranslationEndpoint { get; init; }
    public string Location { get; init; }
}
