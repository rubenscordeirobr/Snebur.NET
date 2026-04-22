using Snebur.Core.Utils;

namespace Snebur.SharedKernel.Localization;

public abstract class JsonStringLocalizerBase : IJsonStringLocalizer
{
    private readonly IJsonStringLocalizerCache _cache;
    private readonly ICultureProvider _cultureProvider;
    private readonly string _resourceKey;

    protected JsonStringLocalizerBase(
        Type resourceType,
        IJsonStringLocalizerCache localizationCache,
        ICultureProvider cultureProvider)
    {
        Guard.NotNull(localizationCache);
        Guard.NotNull(cultureProvider);
        Guard.NotNull(resourceType);

        _resourceKey = LocalizationHelper.GetResourceKey(resourceType);
        _cache = localizationCache;
        _cultureProvider = cultureProvider;
    }

    public string this[string localizationKey, string defaultValue, params object[] args]
        => GetLocalizedString(localizationKey, defaultValue, args);

#pragma warning disable CA1043
    public string this[Enum enumValue]
        => GetEnumLocalizedString(enumValue);

#pragma warning restore CA1043

    private string GetLocalizedString(string localizationKey, string defaultValue, object[] args)
    {
        var language = _cultureProvider.Language;
        var localizationString = _cache.GetLocalizedString(
            language,
            _resourceKey,
            localizationKey,
            defaultValue);

        return StringFormatUtils.Format(localizationString, args);
    }

    private string GetEnumLocalizedString(Enum enumValue)
    {
        Guard.NotNull(enumValue);

        var enumType = enumValue.GetType().GetUnderlyingType();
        var defaultValue = enumValue.GetDescription();
 
        var resourceKey = LocalizationHelper.GetResourceKey(enumType);
        var localizationKey = enumValue.ToString();
        var language = _cultureProvider.Language;

        return _cache.GetLocalizedString(
            language,
            resourceKey,
            localizationKey,
            defaultValue);
    }
}
