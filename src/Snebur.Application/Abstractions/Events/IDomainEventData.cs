namespace Snebur.Application.Abstractions.Events;

public interface IDomainEventData<out TEvent>
    where TEvent : IDomainEvent
{
    IDomainEventContext Context { get; }
    TEvent DomainEvent { get; }

    void Cancel(DomainEventError error);

    void Cancel(string code, string message);
}
