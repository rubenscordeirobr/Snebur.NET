namespace Snebur.Core.Factories;

public static class TypeDefaultValueFactory
{
    public static object? CreateDefaultValue(Type type)
    {
        Guard.NotNull(type);

        return type.IsValueType
            ? Activator.CreateInstance(type)
            : null;
    }

    public static object GetNotNullDefaultValue(Type type)
    {
        Guard.NotNull(type);

        if (type.IsValueType)
        {
            return Activator.CreateInstance(type)!;
        }

        if (type == typeof(string))
        {
            return string.Empty;
        }

        if (type.IsArray)
        {
            return Array.CreateInstance(type.GetElementType()!, 0);
        }

        if (type.IsGenericType)
        {
            var genericDefinition = type.GetGenericTypeDefinition();
            if (genericDefinition == typeof(List<>) ||
                genericDefinition == typeof(Dictionary<,>) ||
                genericDefinition == typeof(HashSet<>))
            {
                return Activator.CreateInstance(type)!;
            }
        }

        if (!type.IsConcrete())
        {
            throw new InvalidCastException(
                $"Cannot create a default non-null value for type '{type.FullName}' because it's not concrete.");
        }

        try
        {
            return Activator.CreateInstance(type)!;
        }
        catch (Exception ex)
        {
            throw new InvalidCastException(
              $"Cannot create a default non-null value for type '{type.FullName}'.", ex);
        }
    }
}

