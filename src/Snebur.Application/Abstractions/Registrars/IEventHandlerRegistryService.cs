namespace Snebur.Application.Abstractions.Registrars;

public interface IEventHandlerRegistryService
{
    void MapperDomainEventHandler(
        Type domainEventType,
        Type handlerType);

    void MapperDomainEventPreProcessorHandler(
        Type domainEventType,
        Type handlerType);

    IReadOnlyList<Type> GetDomainEventHandlers(Type eventType);

    IReadOnlyList<Type> GetDomainEventPreProcessorHandlers(Type eventType);
}
