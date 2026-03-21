using System.Collections;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Snebur.Core.Utils;

public static class JsonUtils
{
    private static JsonSerializerOptions? _cacheJsonSerializerOptions;
    private static JsonSerializerOptions? _langJsonSerializerOptions;

    public static JsonSerializerOptions CacheJsonSerializerOptions
        => (_cacheJsonSerializerOptions ??= CreateCacheJsonSerializerOptions());

    public static JsonSerializerOptions LocalizationJsonSerializerOptions
        => (_langJsonSerializerOptions ??= CreateLangJsonSerializerOptions());

    public static string Serialize(object? obj, JsonSerializerOptions? options = null)
    {
        options ??= JsonSerializerOptions.Web;
        EnableIndentationInDevelopment(options);
        return JsonSerializer.Serialize(obj, options ?? JsonSerializerOptions.Web);
    }

    public static T? Deserialize<T>(string json, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Deserialize<T>(json, options ?? JsonSerializerOptions.Web);
    }

    public static object? Deserialize(string json, Type returnType, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Deserialize(json, returnType: returnType, options ?? JsonSerializerOptions.Web);
    }

    public static void EnableIndentationInDevelopment(JsonSerializerOptions jsonSerializerOptions)
    {
        Guard.NotNull(jsonSerializerOptions);

        if (jsonSerializerOptions.WriteIndented)
            return;

        if (!EnvironmentHelper.IsDevelopment() &&
            !EnvironmentHelper.IsXUnitTesting())
        {
            return;
        }

        if (jsonSerializerOptions.IsReadOnly)
        {
            ReflectionUtils.SetFiledValue(jsonSerializerOptions, "_writeIndented", true);
            return;
        }

        jsonSerializerOptions.WriteIndented = true;
    }

    public static string GetJsonSchemaType(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;

        if (type == typeof(string))
        {
            return "string";
        }

        if (type == typeof(bool))
        {
            return "boolean";
        }

        if (type == typeof(int) || type == typeof(long) || type == typeof(short) ||
            type == typeof(byte) || type == typeof(uint) || type == typeof(ulong) || type == typeof(ushort))
        {
            return "integer";
        }

        if (type == typeof(float) || type == typeof(double) || type == typeof(decimal))
        {
            return "number";
        }

        if (type.IsEnum)
        {
            return "number";
        }

        if (typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string))
        {
            return "array";
        }
        return "object";
    }

    private static JsonSerializerOptions CreateCacheJsonSerializerOptions()
    {
        var writeIndented = EnvironmentHelper.IsDevelopment() || EnvironmentHelper.IsXUnitTesting();
        return new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            IncludeFields = true,
            IgnoreReadOnlyProperties = false,
            WriteIndented = writeIndented,
            IgnoreReadOnlyFields = true,
        };
    }

    private static JsonSerializerOptions CreateLangJsonSerializerOptions()
    {
        return new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
    }

    public static T? DeserializeFile<T>(string fileName, JsonSerializerOptions? options = null)
    {
        if (!File.Exists(fileName))
        {
            throw new FileNotFoundException($"File not found: {fileName}", fileName: fileName);
        }

        var json = File.ReadAllText(fileName, Encoding.UTF8);
        return Deserialize<T>(json, options);

    }
}

