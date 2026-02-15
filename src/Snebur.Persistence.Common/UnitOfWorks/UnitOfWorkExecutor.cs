namespace Snebur.Persistence.Common.UnitOfWorks;

internal class UnitOfWorkExecutor : UnitOfWorkExecutorBase
{
    public UnitOfWorkExecutor(
        DbContext dbContext,
        IHttpContextSessionAccessor userSessionAccessor,
        IEntityAuthorizationService entityAuthorizationService,
        IEventMediator eventMediator,
        ILogger logger)
        : base(dbContext, userSessionAccessor, entityAuthorizationService, eventMediator, logger)
    {
    }

    public override Task<SaveChangesResult> SaveChangesAsync(bool silent, CancellationToken cancellationToken)
    {
        return ExecuteSaveChangesAsync(silent, cancellationToken);
    }

    protected override Task DispatchAsyncDomainEventAsync(
        DomainEventContext domainEventContext,
        int rowAffects )
    {
        return EventMediator.DispatchAsync(domainEventContext);
    }

    protected override Task PreProcessorDispatchAsync(DomainEventContext domainEventContext)
    {
        return EventMediator.PreProcessorDispatchAsync(domainEventContext);
    }
}
