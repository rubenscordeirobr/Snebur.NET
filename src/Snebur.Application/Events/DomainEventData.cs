namespace Snebur.Application.Events;

public sealed class DomainEventData<TEvent> :IDomainEventData<TEvent>
    where TEvent : IDomainEvent
{
    public IDomainEventContext Context { get; }
    public TEvent DomainEvent { get; }

    public DomainEventData(IDomainEventContext context, TEvent domainEvent)
    {
        Guard.NotNull(context);
        Guard.NotNull(domainEvent);

        Context = context;
        DomainEvent = domainEvent;
    }

    public void Cancel(DomainEventError error)
    {
        this.Context.Cancel(error);
    }

    public void Cancel(string code, string message)
    {
        Cancel(new DomainEventError(code, message));
    }
}
