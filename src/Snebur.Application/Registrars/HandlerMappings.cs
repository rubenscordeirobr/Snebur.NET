using System.Collections.Concurrent;

namespace Snebur.Application.Registrars;

internal class HandlerMappings(HandlerKind kind)
{
    private readonly HandlerMappper _mapper = new(kind);
    private readonly ConcurrentDictionary<Type, GenericEventMapper> _entityChagenEventsMapper = new();
   
    internal void MapperEventHandler(
        Type domainEventType,
        Type handlerType)
    {
  
        if (domainEventType.IsGenericType)
        {
            var eventTypeDefinition = domainEventType.GetGenericTypeDefinition();

            TypeGuard.TypeMustImplementInterface(eventTypeDefinition, typeof(IEntityStateChangedEvent<>));

            var genericMapper = _entityChagenEventsMapper.GetOrAdd(eventTypeDefinition, _ => new(kind));

            ValidateGenericHandlerType(handlerType);

            genericMapper.Map(domainEventType, handlerType);
        }
        else
        {
            _mapper.Map(domainEventType, handlerType);
        }
    }
     
    internal IReadOnlyList<Type> GetHandlerTypes(Type eventType)
    {
        var handlerTypes = new List<Type>();
        var assignableTypes = eventType.GetAssignableTypes();
        foreach (var type in assignableTypes)
        {
            handlerTypes.AddRange(GetHandlerTypesInternal(type));
        }
        return handlerTypes
                .Distinct()
                .ToList();
    }

    private IEnumerable<Type> GetHandlerTypesInternal(Type type)
    {
        if (type.IsGenericType)
        {
            var genericTypeDefinition = type.GetGenericTypeDefinition();
            if (_entityChagenEventsMapper.TryGetValue(genericTypeDefinition, out var genericMapper))
            {
                return genericMapper.GetHandlers(type);
            }
        }
        else
        {
            return _mapper.GetHandler(type);
        }
        return [];
    }
    private void ValidateGenericHandlerType(Type handlerType)
    {
        if (kind == HandlerKind.DomainEventHandler)
        {
            TypeGuard.TypeMustImplementInterface(handlerType, typeof(IEntityStateChangedEventHandler<>));
        }
        else if (kind == HandlerKind.PreProcessorHandler)
        {
            TypeGuard.TypeMustImplementInterface(handlerType, typeof(IEntityStateChangedEventPreProcessorHandler<>));
        }
    }
}
 
