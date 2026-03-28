using Snebur.Application.Events;

namespace Snebur.RuntimeServices.Mediators;

internal static class DomainEventDataFactory
{
    internal static object Create(
        IDomainEventContext eventContext,
        IDomainEvent domainEvent)
    {
        var type = typeof(DomainEventData<>)
            .MakeGenericType(domainEvent.GetType());

        var instance = Activator.CreateInstance(type, eventContext, domainEvent);
        if (instance == null)
        {
            throw new InvalidOperationException($"Failed to create DomainEventData instance {type}");
        }
        return instance;
    }

    internal static DomainEventData<TEvent> Create<TEvent>(
       IDomainEventContext eventContext,
       TEvent domainEvent)
       where TEvent : IDomainEvent
    {
        return new DomainEventData<TEvent>(eventContext, domainEvent);
    }
}
