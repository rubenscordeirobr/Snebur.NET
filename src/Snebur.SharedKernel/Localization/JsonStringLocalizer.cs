namespace Snebur.SharedKernel.Localization;

public class JsonStringLocalizer<T> : JsonStringLocalizerBase, IJsonStringLocalizer<T>
{
    public JsonStringLocalizer(
        IJsonStringLocalizerCache localizationCache,
        ICultureProvider cultureProvider)
        : base(typeof(T), localizationCache, cultureProvider)
    {
    }
}

public class JsonStringLocalizer : JsonStringLocalizerBase
{
    public JsonStringLocalizer(
        Type resourceType,
        IJsonStringLocalizerCache localizationCache,
        ICultureProvider cultureProvider)
        : base(resourceType, localizationCache, cultureProvider)
    {
    }
}
