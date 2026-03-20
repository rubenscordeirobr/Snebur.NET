using Snebur.Application.Abstractions.Handlers;

namespace Snebur.Testing.Core.Extensions;

public static class DomainEventContextExtensions
{
    public static ExecutedEventContext<TEvent> ShouldHaveExecutedEvent<TEvent>(
        this DomainEventContext context,
        TEvent domainEvent)
        where TEvent : IDomainEvent
    {
        Guard.NotNull(context);

        var executedEvents = context.GetExecutedEventResults(domainEvent);
        executedEvents.Should()
            .Contain(x => x.DomainEvent.Equals(domainEvent));

        return new ExecutedEventContext<TEvent>(executedEvents);
    }
}

public class ExecutedEventContext<TEvent>
    where TEvent : IDomainEvent
{
    private readonly IReadOnlyList<ExecutedDomainEventResult> _executedEvents;

    public ExecutedEventContext(
        IReadOnlyList<ExecutedDomainEventResult> executedEvents)
    {
        _executedEvents = executedEvents;
    }

    public ExecutedEventContext<TEvent> WithHandler<THandler>()
        where THandler : IApplicationHandler
    {
        var handlerType = typeof(THandler);

        _executedEvents.Should()
            .ContainSingle(x => x.HandlerType == handlerType
                            || x.ImplementationHandlerType == handlerType);
        return this;
    }
}
