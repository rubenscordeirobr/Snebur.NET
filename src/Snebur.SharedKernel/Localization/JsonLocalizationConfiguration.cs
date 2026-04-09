namespace Snebur.SharedKernel.Localization;

public record JsonLocalizationConfiguration : JsonLocalizationCacheConfiguration
{
    public required string ResourcesRootPath { get; init; }
}
