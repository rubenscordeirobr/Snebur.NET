using Snebur.Application.Abstractions.Handlers;

namespace Snebur.Application.Abstractions.Events;

public interface IPreProcessorHandler<in TEvent> :IApplicationHandler
    where TEvent : IDomainEvent
{
    Task PreProcessAsync(
        IDomainEventData<TEvent> eventData,
        CancellationToken cancellationToken = default);

    Task IApplicationHandler.HandleAsync(object handlerObject, CancellationToken cancellationToken)
    {
        return PreProcessAsync((IDomainEventData<TEvent>)handlerObject, cancellationToken);
    }
}
