using System.ComponentModel;
using System.Reflection;

namespace Snebur.Core.Extensions;

public static class EnumExtensions
{
    public static bool IsSystemValue(this Enum value)
    {
        Guard.NotEmpty(value);
        return value.GetCustomAttribute<SystemValueAttribute>() != null;
    }

    public static string GetDescription(this Enum value)
    {
        Guard.NotNull(value);

        var attribute = value.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? value.ToString();
    }

    public static TAttribute? GetCustomAttribute<TAttribute>(this Enum value)
        where TAttribute : Attribute
    {
        Guard.NotNull(value);

        var field = value.GetType()
            .GetField(value.ToString());

        if (field is null)
            return null;

        return field.GetCustomAttribute<TAttribute>();
    }

}

