using System.Reflection;

namespace Snebur.Core.Utils;

public static class ReflectionUtils
{

    public const BindingFlags AllInstanceBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    public static object? GetPropertyValue(object obj, string propertyName)
    {
        Guard.NotNull(obj);
        Guard.NotNullOrWhiteSpace(propertyName);

        var property = obj.GetType().GetProperty(propertyName, AllInstanceBindingFlags);
        if (property is null)
        {
            throw new MissingMemberException(
                $"Property '{propertyName}' not found in type '{obj.GetType().FullName}'.");
        }
        return property.GetValue(obj);
    }

    public static object? GetFieldValue(object obj, string fieldName)
    {
        Guard.NotNull(obj);
        Guard.NotNullOrWhiteSpace(fieldName);

        var field = obj.GetType().GetField(fieldName, AllInstanceBindingFlags);
        if (field is null)
        {
            throw new MissingFieldException(
                $"Property '{fieldName}' not found in type '{obj.GetType().FullName}'.");
        }
        return field.GetValue(obj);
    }

    public static void SetFiledValue(
        object targetInstance,
        string fieldName,
        object value)
    {
        Guard.NotNull(targetInstance);
        Guard.NotNullOrWhiteSpace(fieldName);

        var field = targetInstance.GetType().GetField(fieldName, AllInstanceBindingFlags);
        if (field is null)
        {
            throw new MissingFieldException(
                $"Field '{fieldName}' not found in type '{targetInstance.GetType().FullName}'.");
        }
        field.SetValue(targetInstance, value);
    }
}
