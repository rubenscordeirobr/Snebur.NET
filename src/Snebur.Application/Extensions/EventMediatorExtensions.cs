using Snebur.Application.Abstractions.Mediators;
using Snebur.Application.Events;

namespace Snebur.Application.Extensions;

public static class EventMediatorExtensions
{
    public static Task DispatchAsync(this IEventMediator mediator,
        IUserSession userSession,
        IDomainEvent domainEvent)
    {
        Guard.NotNull(mediator);
        Guard.NotNull(domainEvent);

        var domainEventContext = new DomainEventContext(
            userSession,
            [domainEvent]);

        return mediator.DispatchAsync(domainEventContext);
    }
}

