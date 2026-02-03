using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Snebur.Core.Extensions;

public static class TypeExtensions
{
    public static bool IsConcrete(this Type type)
    {
        Guard.NotNull(type);

        return !type.IsAbstract && !type.IsInterface;
    }

    public static bool IsSubclassOf<T>(this Type type)
    {
        Guard.NotNull(type);

        return type.IsSubclassOf(typeof(T));
    }

    public static bool IsSubclassOfOrEquals<T>(this Type type)
    {
        return type.IsSubclassOfOrEquals(typeof(T));
    }
    public static bool IsSubclassOfOrEquals(this Type type, Type otherType)
    {
        Guard.NotNull(type);

        return type.IsSubclassOf(otherType) || type == otherType;
    }

    public static bool ImplementsGenericInterfaceDefinition(
        this Type type,
        Type definitionType)
    {
        Guard.NotNull(type);
        Guard.NotNull(definitionType);

        if (!definitionType.IsGenericType)
        {
            return false;
        }

        if (!definitionType.IsGenericTypeDefinition)
        {
            definitionType = definitionType.GetGenericTypeDefinition();
        }

        if (type.IsGenericType &&
            type.GetGenericTypeDefinition() == definitionType)
        {
            return true;
        }

        return type.GetInterfaces()
            .Any(interfaceType => interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == definitionType);
    }

    public static IEnumerable<PropertyInfo> GetDeclaredProperties(
        this Type type,
        bool isIgnoreNotMappedAtribute = true)
    {
        Guard.NotNull(type);

        var properties = type.GetProperties()
            .Where(x => x.DeclaringType == type);

        return (isIgnoreNotMappedAtribute)
            ? properties.Where(property => property.GetCustomAttribute<NotMappedAttribute>() == null)
            : properties;
    }

    public static IEnumerable<PropertyInfo> GetDeclaredPropertiesOfType<T>(
        this Type type, bool isIgnoreNotMappedAtribute = true)
    {
        return GetDeclaredPropertiesOfType(type, typeof(T), isIgnoreNotMappedAtribute);
    }

    public static IEnumerable<PropertyInfo> GetDeclaredPropertiesOfType(
        this Type type,
        Type propertyType,
        bool ignoreNotMappedAttribute = true)
    {
        Guard.NotNull(type);

        var properties = type.GetProperties()
            .Where(x => x.DeclaringType == type)
            .Where(x => x.PropertyType.IsSubclassOfOrEquals(propertyType));

        return (ignoreNotMappedAttribute)
            ? properties.Where(property => property.GetCustomAttribute<NotMappedAttribute>() == null)
            : properties;
    }

    public static IEnumerable<Type> GetAssignableTypes(this Type eventType)
    {
        Guard.NotNull(eventType);

        var assignableTypes = new List<Type> { eventType };
        assignableTypes.AddRange(eventType.GetInterfaces());
        if (eventType.BaseType != null && eventType.BaseType != typeof(object))
        {
            assignableTypes.Add(eventType.BaseType);
            assignableTypes.AddRange(eventType.BaseType.GetAssignableTypes());
        }
        return assignableTypes;
    }

    public static string GetQualifiedName(this Type type)
    {
        Guard.NotNull(type);

        if (type.IsGenericType)
        {
            var genericArguments = type.GetGenericArguments()
                .Select(GetQualifiedName);

            return $"{type.Name.Split('`')[0]}<{string.Join(", ", genericArguments)}>";
        }
        if (type.FullName is not null)
            return type.FullName;

        if (type.Namespace is not null)
        {
            return string.IsNullOrWhiteSpace(type.Namespace)
            ? type.Name
            : $"{type.Namespace}.{type.Name}";
        }
        return type.Name;
    }

    public static IDictionary<string, PropertyInfo> GetPropertiesFromInterface<TInterface>(
        this Type type)

    {
        return GetPropertiesFromInterface(type, typeof(TInterface));
    }

    public static IDictionary<string, PropertyInfo> GetPropertiesFromInterface(
         this Type type,
        Type interfaceType)

    {
        Guard.NotNull(type);
        Guard.NotNull(interfaceType);

        if (!type.IsAssignableTo(interfaceType))
        {
            var message = $"The type {type.Name} does not implement the interface {interfaceType.Name}";
            throw new InvalidOperationException(message);
        }

        var interfaceProperties = interfaceType.GetProperties();
        var interfacePropertyNames = new HashSet<string>(
           interfaceType.GetProperties().Select(p => p.Name));

        static int GetInheritanceDistance(Type derivedType, Type? declaringType)
        {
            int distance = 0;
            var currentType = derivedType;
            while (currentType != null && currentType != declaringType)
            {
                distance++;
                currentType = currentType.BaseType;
            }
            return currentType == declaringType ? distance : int.MaxValue;
        }

        var interfacePropertyMappings = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => interfacePropertyNames.Contains(p.Name))
            .GroupBy(p => p.Name)
            .ToDictionary(
                group => group.Key,
                group => group.OrderBy(p => GetInheritanceDistance(type, p.DeclaringType)).First()
            );

        if (interfaceProperties.Any(p => !interfacePropertyMappings.ContainsKey(p.Name)))
        {
            var message = $"The properties {string.Join(", ", interfaceProperties.Select(p => p.Name))} from interface {interfaceType.Name} are not implemented in {type.Name}";
            throw new InvalidOperationException(message);
        }

        return interfacePropertyMappings;
    }

    public static string GetSingleName(this Type type)
    {
        Guard.NotNull(type);

        if (type.IsGenericType)
        {
            return type.Name.Substring(0, type.Name.IndexOf('`', StringComparison.Ordinal));
        }
        return type.Name;
    }

    public static bool IsAssignableTo<T>(this Type? type)
    {
        if (type is null)
            return false;

        return type.IsAssignableTo(typeof(T));
    }

    public static bool IsAssignableTo(
        this Type? type,
        IEnumerable<Type> types)
    {
        if (type is null)
            return false;

        return types.Any(type.IsAssignableTo);
    }

    public static bool IsAssignableToOrDefinition(
        this Type? type,
        IEnumerable<Type> types)
    {
        if (type == null)
        {
            return false;
        }
        return types.Any(type.IsAssignableToOrDefinition);
    }

    public static bool IsAssignableToOrDefinition(
        this Type type,
        Type targetType)
    {
        Guard.NotNull(type);
        Guard.NotNull(targetType);

        if (targetType.IsGenericTypeDefinition)
        {
            return type.ImplementsGenericInterfaceDefinition(targetType);
        }
        return type.IsAssignableTo(targetType);
    }

    public static Type GetGenericArgumentFromInterfaceDefinition(
        this Type type,
        Type interfaceDefinition)
    {
        var arguments = type.GetGenericArgumentsFromInterfaceDefinition(interfaceDefinition);
        if (arguments.Length != 1)
        {
            throw new ArgumentException(
                $"The interface {interfaceDefinition.Name} must have exactly one generic argument");
        }
        return arguments[0];
    }

    public static Type[] GetGenericArgumentsFromInterfaceDefinition(
      this Type type,
      Type interfaceDefinition)
    {
        Guard.NotNull(type);
        Guard.NotNull(interfaceDefinition);

        if (!interfaceDefinition.IsGenericTypeDefinition)
        {
            throw new ArgumentException("The interface definition must be a generic type definition", nameof(interfaceDefinition));
        }

        if (!type.ImplementsGenericInterfaceDefinition(interfaceDefinition))
        {
            throw new ArgumentException($"The type {type.Name} does not implement the interface {interfaceDefinition.Name}");
        }

        var interfaces = type.GetInterfaces()
            .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceDefinition)
            .ToArray();

        if (interfaces.Length == 0)
        {
            throw new ArgumentException(
                $"The type {type.Name} does not implement the interface {interfaceDefinition.Name}");
        }

        if (interfaces.Length > 1)
        {
            throw new ArgumentException(
                $"The type {type.Name} implements the interface {interfaceDefinition.Name} more than once");
        }

        return interfaces[0].GetGenericArguments();
    }

    public static Type GetUnderlyingType(this Type type)
    {
        Guard.NotNull(type);

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            return Nullable.GetUnderlyingType(type)
                ?? type;
        }
        return type;
    }

    public static PropertyInfo? GetPropertyByName(
        this Type type, string propertyName)
    {
        Guard.NotNull(type);

        var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase;
        return type.GetProperty(propertyName, flags);
    }

    public static PropertyInfo GetRequiredProperty
        (this Type type, string propertyName)
    {
        var property = type.GetPropertyByName(propertyName);
        if (property is null)
        {
            throw new InvalidOperationException($"The property {propertyName} was not found in the type {type.Name}");
        }
        return property;
    }

    public static string GetDisplayName(this Type type, bool excludeNestedTypeNames = false)
    {
        Guard.NotNull(type);

        if (type.IsGenericType)
        {
            var genericArguments = type.GetGenericArguments()
                .Select(x => GetDisplayName(x, excludeNestedTypeNames));

            return $"{type.Name.Split('`')[0]}<{string.Join(", ", genericArguments)}>";
        }

        if (!excludeNestedTypeNames && type.IsNested && type.DeclaringType is not null)
        {
            return $"{type.DeclaringType.Name}.{type.Name}";
        }
        return type.Name;
    }
}
