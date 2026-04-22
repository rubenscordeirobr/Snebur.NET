using Snebur.SharedKernel.Abstractions;

namespace Snebur.SharedKernel.Extensions;

public static class EnumExtensions
{
    public static string GetLocalizedDescription<TEnum>(
        this TEnum value,
        IJsonStringLocalizer<TEnum> localizer) where TEnum : Enum
    {
        Guard.NotNull(localizer);

        
        var defaultValue = value.GetDescription();
        var resourceKey = value.ToString();
        return localizer[resourceKey, defaultValue];
    }
}
