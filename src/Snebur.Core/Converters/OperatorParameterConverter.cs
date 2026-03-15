using System.Globalization;
using System.Reflection;

namespace Snebur.Core.Converters;

public static class OperatorParameterConverter
{
    public static string? ToString(object value)
    {
        if (value is null)
        {
            return null;
        }

        var type = value.GetType();
        return ToString(value, type);
    }
     
    public static string? ToString(object? value, Type type)
    {
        Guard.NotNull(type);

        if (value is null)
        {
            return null;
        }

        type = type.GetUnderlyingType();

        if (type == typeof(double))
        {
            return ((double)value).ToString(CultureInfo.InvariantCulture);
        }

        if (type == typeof(float))
        {
            return ((float)value).ToString(CultureInfo.InvariantCulture);
        }
        if (type == typeof(decimal))
        {
            return ((decimal)value).ToString(CultureInfo.InvariantCulture);
        }

        if (type == typeof(DateTime))
        {
            return ((DateTime)value).ToString("O", CultureInfo.InvariantCulture);
        }

        if (type == typeof(TimeSpan))
        {
            return ((TimeSpan)value).ToString("c", CultureInfo.InvariantCulture);
        }
        return Convert.ToString(value, CultureInfo.InvariantCulture);
    }

    public static object? Parse(string? stringValue, ParameterInfo parameter)
    {
        Guard.NotNull(parameter);

        var resolverAttribute = parameter.GetCustomAttribute<ParameterParserResolverAttribute>();
        if (resolverAttribute != null)
        {
            return resolverAttribute.Parse(stringValue);
        }

        var value = Parse(stringValue, parameter.ParameterType);
        if (value is null && !parameter.IsNullable())
        {
            return TypeDefaultValueFactory.GetNotNullDefaultValue(parameter.ParameterType);
        }
        return value;
    }

    public static object? Parse(string? value, Type type)
    {
        Guard.NotNull(type);

        var isNullable = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

        if (string.IsNullOrEmpty(value) || value == "null")
        {
            return isNullable
                ? null
                : TypeDefaultValueFactory.CreateDefaultValue(type);
        }

        if (isNullable)
        {
            type = type.GetUnderlyingType();
        }

        if (type == typeof(bool))
        {
            return bool.Parse(value);
        }
        if (type == typeof(int))
        {
            return int.Parse(value, CultureInfo.InvariantCulture);
        }
        if (type == typeof(long))
        {
            return long.Parse(value, CultureInfo.InvariantCulture);
        }
        if (type == typeof(short))
        {
            return short.Parse(value, CultureInfo.InvariantCulture);
        }

        if (type == typeof(string))
        {
            return value;
        }

        if (type == typeof(double))
        {
            return double.Parse(value, CultureInfo.InvariantCulture);
        }

        if (type == typeof(float))
        {
            return float.Parse(value, CultureInfo.InvariantCulture);
        }

        if (type == typeof(decimal))
        {
            return decimal.Parse(value, CultureInfo.InvariantCulture);
        }

        if (type == typeof(Guid))
        {
            return Guid.Parse(value);
        }

        if (type == typeof(DateTime))
        {
            return DateTime.Parse(value, CultureInfo.InvariantCulture);
        }

        if (type == typeof(TimeSpan))
        {
            return TimeSpan.Parse(value, CultureInfo.InvariantCulture);
        }

        if (type.IsEnum)
        {
            return Enum.Parse(type, value, ignoreCase: true);
        }

        // Fallback to ChangeType for other convertible types.
        return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
    }
}
