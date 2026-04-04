using Snebur.Application.Abstractions.Handlers;

namespace Snebur.Application.Abstractions.Events;

public interface IDomainEventHandler<in TEvent> : IApplicationHandler
    where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);

    Task IApplicationHandler.HandleAsync(object handlerObject, CancellationToken cancellationToken)
    {
        return HandleAsync((TEvent)handlerObject, cancellationToken);
    }
}
