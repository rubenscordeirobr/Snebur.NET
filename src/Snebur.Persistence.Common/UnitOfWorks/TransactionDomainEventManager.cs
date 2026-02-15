namespace Snebur.Persistence.Common.UnitOfWorks;

internal class TransactionDomainEventManager(IUserSession userSession)
{
    internal readonly List<DomainEventContext> _domainEventContexts = new();

    internal DomainEventContext GetDomainEventContext()
        => CreateDomainEventContext();

    internal void Add(DomainEventContext domainEventContext)
    {
        _domainEventContexts.Add(domainEventContext);
    }

    private DomainEventContext CreateDomainEventContext()
    {
        var events = _domainEventContexts
            .SelectMany(domainEventContext => domainEventContext.Events)
            .Distinct()
            .ToList();

        return new DomainEventContext(userSession, events);
    }
}
