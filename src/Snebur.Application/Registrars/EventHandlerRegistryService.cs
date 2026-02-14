using Snebur.Application.Abstractions.Registrars;

namespace Snebur.Application.Registrars;

internal class EventHandlerRegistryService : IEventHandlerRegistryService
{
    private readonly HandlerMappings _domainEventHandlerMappings = new(HandlerKind.DomainEventHandler);
    private readonly HandlerMappings _domainEventPreProcessorHandlerMappings = new(HandlerKind.PreProcessorHandler);

    public void MapperDomainEventHandler(
        Type domainEventType,
        Type handlerType)
    {
        _domainEventHandlerMappings.MapperEventHandler(domainEventType, handlerType);
    }

    public void MapperDomainEventPreProcessorHandler(
        Type domainEventType,
        Type handlerType)
    {
        _domainEventPreProcessorHandlerMappings.MapperEventHandler(domainEventType, handlerType);
    }

    public IReadOnlyList<Type> GetDomainEventHandlers(Type eventType)
    {
        return _domainEventHandlerMappings.GetHandlerTypes(eventType);
    }

    public IReadOnlyList<Type> GetDomainEventPreProcessorHandlers(Type eventType)
    {
        return _domainEventPreProcessorHandlerMappings.GetHandlerTypes(eventType);
    }
}
