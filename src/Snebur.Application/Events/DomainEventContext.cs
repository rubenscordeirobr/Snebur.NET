using System.Diagnostics.CodeAnalysis;

namespace Snebur.Application.Events;

public sealed class DomainEventContext : IDomainEventContext
{
    private readonly Dictionary<IDomainEvent, List<ExecutedDomainEventResult>> _dispatchedHandlers = [];

    private bool _canBeCanceled = true;
    public IReadOnlyList<IDomainEvent> Events { get; }

    [MemberNotNullWhen(true, nameof(Error))]
    [MemberNotNullWhen(true, nameof(Exception))]
    public bool IsCanceled { get; private set; }

    public IUserSession UserSession { get; }

    public DomainEventError? Error { get; private set; }

    public DomainEventContext(
        IUserSession userSession, 
        IEnumerable<IDomainEvent> events)
    {
        Guard.NotNull(userSession);
        Guard.NotNull(events);

        UserSession = userSession;
        Events = events.ToList();
    }

    public void Cancel(DomainEventError error)
    {
        if (!_canBeCanceled)
        {
            throw new InvalidOperationException("The context cannot be canceled.");
        }

        Guard.NotNull(error);
        IsCanceled = true;
        Error = error;
    }

    public void LockCancellation()
    {
        _canBeCanceled = false;
    }

    public Exception? Exception
        => IsCanceled
            ? new DomainEventException(Error.Message)
            : null;

    public void AddExecutedEventResults(
        IDomainEvent domainEvent,
        IReadOnlyList<ExecutedDomainEventResult> results)
    {
        _dispatchedHandlers.TryAdd(domainEvent, []);
        if (_dispatchedHandlers.TryGetValue(domainEvent, out var value))
        {
            value.AddRange(results);
        }
        else
        {
            _dispatchedHandlers.Add(domainEvent, [.. results]);
        }
    }

    public IReadOnlyList<ExecutedDomainEventResult> GetExecutedEventResults(IDomainEvent domainEvent)
    {
        if (!_dispatchedHandlers.TryGetValue(domainEvent, out var value))
        {
            return [];
        }
        return value;
    }
}
