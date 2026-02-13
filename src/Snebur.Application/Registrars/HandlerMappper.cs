using System.Collections.Concurrent;

namespace Snebur.Application.Registrars;

internal class HandlerMappper(HandlerKind kind)
{
    private readonly ConcurrentDictionary<Type, HashSet<Type>> _mappings = new();

    public void Map(Type type, Type handlerType)
    {
        if (type.ContainsGenericParameters)
        {
            throw new InvalidOperationException("Event type must not be nested");
        }

        if (!type.IsSubclassOfOrEquals<EntityBase>())
        {
            TypeGuard.TypeMustImplementInterface(type, typeof(IDomainEvent));
        }
        ValidateHandlerType(handlerType);

        var handlers = _mappings.GetOrAdd(type, _ => []);
        lock (handlers)
        {
            handlers.Add(handlerType);
        }
    }

    internal IEnumerable<Type> GetHandler(Type eventType)
    {
        var handlerTypes = new List<Type>();
        foreach (var type in eventType.GetAssignableTypes())
        {
            if (_mappings.TryGetValue(type, out var handlers))
            {
                lock (handlers)
                {
                    handlerTypes.AddRange(handlers);
                }
            }
        }
        return handlerTypes;
    }

    private void ValidateHandlerType(Type handlerType)
    {
        if (kind == HandlerKind.DomainEventHandler)
        {
            TypeGuard.TypeMustImplementInterface(handlerType, typeof(IDomainEventHandler<>));
        }
        else if (kind == HandlerKind.PreProcessorHandler)
        {
            TypeGuard.TypeMustImplementInterface(handlerType, typeof(IPreProcessorHandler<>));
        }
    }
}
