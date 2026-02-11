using System.Runtime.CompilerServices;

namespace Snebur.Core;

public static class TypeGuard
{
    public static void MustBeNotGeneric(Type type,
          [CallerArgumentExpression("type")] string? paramName = "")
    {
        Guard.NotNull(type);

        if (type.IsGenericType)
            throw new InvalidOperationException($"{paramName}: Type{type.Name} is a generic type");
    }

    public static void MustBeConcrete(Type type,
        [CallerArgumentExpression("type")] string? paramName = "")
    {
        Guard.NotNull(type);

        if (!type.IsConcrete())
            throw new InvalidOperationException($"{paramName}: Type{type.Name} must be a concrete type");
    }

    public static void TypeMustBeAssignableFrom(Type type, Type other)
    {
        Guard.NotNull(type);
        Guard.NotNull(other);

        if (!other.IsAssignableFrom(type))
            throw new InvalidOperationException($"{type.Name} must be a subclass of {other.Name}");

    }

    public static void TypeMustBeSubclassOfOrEqual(Type type, Type other)
    {
        Guard.NotNull(type);
        Guard.NotNull(other);

        if (!type.IsSubclassOfOrEquals(other))
            throw new InvalidOperationException($"{type.Name} must be a subclass of {other.Name}");

    }

    public static void TypeMustBeInterface(Type type, Type other)
    {
        Guard.NotNull(type);
        Guard.NotNull(other);

        if (!other.IsInterface)
            throw new InvalidOperationException($"{other.Name} must be an interface");
    }

    public static void TypeMustImplementInterface(Type type, Type interfaceType)
    {
        Guard.NotNull(type);
        Guard.NotNull(interfaceType);

        if (type.ImplementsGenericInterfaceDefinition(interfaceType))
        {
            return;
        }

        if (!interfaceType.IsAssignableFrom(type))
        {
            throw new InvalidOperationException($"{type.Name} must implement {interfaceType.Name}");
        }
    }
}
