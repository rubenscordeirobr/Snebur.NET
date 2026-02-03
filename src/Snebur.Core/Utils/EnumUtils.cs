using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Snebur.Core.Utils;

public static class EnumUtils
{
    public static T[] GetSelectableEnumValues<T>()
       where T : struct, Enum
    {
        var values = Enum.GetValues<T>()
            .Where(value => ShouldFilterValue(value, skipUndefined: false, skipSystemValue: true))
            .ToArray();
        return values;
    }

    public static T[] GetValues<T>(bool skipUndefined = true, bool skipSystemValue = false)
        where T : struct, Enum
    {
        var values = Enum.GetValues<T>()
            .Where(value => ShouldFilterValue(value, skipUndefined, skipSystemValue))
            .ToArray();

        return values;
    }

    public static bool TryParse<TEnum>(
        [NotNullWhen(true)] string? value, out TEnum result)
        where TEnum : struct, Enum
    {
        if (Enum.TryParse(value, true, out TEnum parsedValue) &&
            Enum.IsDefined(parsedValue))
        {
            result = parsedValue;
            return true;
        }
        result = default;
        return false;
    }

    public static TEnum Parse<TEnum>(
        [NotNullWhen(true)] string? value)
        where TEnum : struct, Enum
    {
        if (TryParse(value, out TEnum result))
        {
            return result;
        }
        throw new ArgumentException($"Value '{value}' is not a valid or defined in {typeof(TEnum).Name}.");
    }

    public static bool IsDefined(Type enumType, object value)
    {
        Guard.NotNull(enumType);

        if (!Enum.IsDefined(enumType, value) || value is null)
        {
            return false;
        }

        var member = enumType.GetMember(value.ToString()!, MemberTypes.Field, BindingFlags.Public | BindingFlags.Static)
               .FirstOrDefault();
        if (member is null)
            return false;

        return member.GetCustomAttribute<UndefinedValueAttribute>() is null;
    }

    public static bool IsDefined<TEnum>(TEnum value)
        where TEnum : struct, Enum
    {
        if (!Enum.IsDefined(value))
        {
            return false;
        }
        return ShouldFilterValue(value, skipUndefined: true);
    }

    public static T Random<T>()
        where T : struct, Enum
    {
        var random = new Random();
        var values = Enum.GetValues<T>();
#pragma warning disable CA5394 
        var randomIndex = random.Next(0, values.Length);
#pragma warning restore CA5394 
        return values[randomIndex];
    }

    private static bool ShouldFilterValue<TEnum>(
        TEnum value,
        bool skipUndefined = true,
        bool skipSystemValue = false)
        where TEnum : struct, Enum
    {
        if (skipUndefined)
        {
            if (value.GetCustomAttribute<UndefinedValueAttribute>() is not null)
                return false;
        }

        if (skipSystemValue)
        {
            return value.GetCustomAttribute<SystemValueAttribute>() is null;
        }
        return true;
    }

}
