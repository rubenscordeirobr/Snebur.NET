namespace Snebur.Application.Registrars;

internal class GenericEventMapper(HandlerKind kind)
{
    private readonly Dictionary<Type, HandlerMappper> _genericsHandlers = new();

    internal void Map(Type eventType, Type handlerType)
    {
        Guard.NotNull(eventType);
        Guard.NotNull(handlerType);

        var genericArguments = eventType.GetGenericArguments();
        if (genericArguments.Length != 1)
        {
            throw new InvalidOperationException("Event type must have only one generic argument");
        }

        var genericEntityParameterType = genericArguments[0];
        var entityType = NormalizarGerenicType(eventType, genericEntityParameterType);

        TypeGuard.TypeMustBeSubclassOfOrEqual(entityType, typeof(EntityBase));

        var mapper = _genericsHandlers
            .GetOrAdd(entityType, () => new(kind));

        mapper.Map(entityType, handlerType);
    }

    private Type NormalizarGerenicType(Type eventType, Type genericParameterType)
    {
        if (genericParameterType.ContainsGenericParameters)
        {
            var constraintsTypes = genericParameterType.GetGenericParameterConstraints();
            if (constraintsTypes.Length > 1)
            {
                throw new InvalidOperationException($"The event type {eventType.Name} has generic parameter {genericParameterType.Name} with more than one constraint");
            }

            if (constraintsTypes.Length == 1)
            {
                return constraintsTypes[0];
            }
        }

        return genericParameterType;
    }

    internal IEnumerable<Type> GetHandlers(Type eventType)
    {
        Guard.NotNull(eventType);

        var genericArguments = eventType.GetGenericArguments();
        if (genericArguments.Length != 1)
        {
            throw new InvalidOperationException("Event type must have only one generic argument");
        }

        var genericParameter = eventType.GetGenericArguments()[0];
        var genericType = NormalizarGerenicType(eventType, genericParameter);
        var handlerTypes = new List<Type>();

        foreach (var assinableType in genericType.GetAssignableTypes())
        {
            if (_genericsHandlers.TryGetValue(assinableType, out var handlerMapper))
            {
                handlerTypes.AddRange(handlerMapper.GetHandler(assinableType));
            }
        }
        return handlerTypes;
    }
}
