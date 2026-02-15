using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Snebur.Persistence.Common.UnitOfWorks;

internal abstract class UnitOfWorkExecutorBase
{
    private readonly DbContext _dbContext;
    private readonly IHttpContextSessionAccessor _userSessionAccessor;
    private readonly IEntityAuthorizationService _entityAuthorizationService;

    protected ILogger Logger { get; }
    protected IEventMediator EventMediator { get; }

    protected UnitOfWorkExecutorBase(
        DbContext dbContext,
        IHttpContextSessionAccessor userSessionAccessor,
        IEntityAuthorizationService entityAuthorizationService,
        IEventMediator eventMediator,
        ILogger logger)
    {
        _dbContext = dbContext;
        _userSessionAccessor = userSessionAccessor;
        _entityAuthorizationService = entityAuthorizationService;
        Logger = logger;

        EventMediator = eventMediator;
    }

    protected async Task<SaveChangesResult> ExecuteSaveChangesAsync(
        bool silent,
        CancellationToken cancellationToken)
    {
        var entries = _dbContext.ChangeTracker
              .Entries<EntityBase>()
              .Where(x => x.HasChanges())
              .ToList();

        var userSession = _userSessionAccessor.GetRequiredUserSession();
        var domainEventContext = DomainEventContextFactory.Create(userSession, entries);

        try
        {
            ValidateAndSetUserSessions(userSession, entries);

            await PreProcessorDispatchAsync(domainEventContext);

            domainEventContext.LockCancellation();

            if (domainEventContext.IsCanceled)
            {
                Logger.LogError("Domain event context was canceled. {Message}.", domainEventContext.Error.Message);

                if (!silent)
                {
                    throw new DomainEventException(
                        $"Domain event context WAS canceled. {domainEventContext.Error.Message}");
                }
                return SaveChangesResult.DomainEventError(domainEventContext, domainEventContext.Error);
            }

            if (cancellationToken.IsCancellationRequested)
            {
                Logger.LogWarning("Operation canceled during save changes.");

                if (!silent)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }
                return SaveChangesResult.OperationCanceledError(domainEventContext, cancellationToken);
            }

            var rowAffects = await _dbContext.SaveChangesAsync(cancellationToken);
            await DispatchAsyncDomainEventAsync(domainEventContext, rowAffects);
            return SaveChangesResult.Success(domainEventContext, rowAffects);

        }
        catch (ForbiddenSecurityException ex)
        {
            Logger.LogError(ex,
                "Unauthorized security exception during save changes.");

            if (!silent)
            {
                throw;
            }

            var unauthorizedError = new ForbiddenError("UnitOfWork.SaveChanges", ex.Message);
            return SaveChangesResult.UnauthorizedError(
                domainEventContext, unauthorizedError);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during save changes.");

            if (!silent)
            {
                throw;
            }
            return SaveChangesResult.SaveChangesError(ex, domainEventContext);
        }
    }

    protected abstract Task PreProcessorDispatchAsync(DomainEventContext domainEventContext);

    protected abstract Task DispatchAsyncDomainEventAsync(
        DomainEventContext domainEventContext,
        int rowAffects);

    public abstract Task<SaveChangesResult> SaveChangesAsync(bool silent, CancellationToken cancellationToken);

    private void ValidateAndSetUserSessions(
        IUserSession userSession,
        List<EntityEntry<EntityBase>> entries)
    {
        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:

                    ValidateEntityChange(entry.Entity, userSession, EntityChangeState.Created);

                    entry.Entity.SetCreateSession(userSession.Id);
                    break;
                case EntityState.Modified:

                    ValidateEntityChange(entry.Entity, userSession, EntityChangeState.Updated);
                    entry.Entity.SetUpdateSession(userSession.Id);
                    break;
                case EntityState.Deleted:

                    ValidateEntityChange(entry.Entity, userSession, EntityChangeState.Deleted);

                    if (entry.Entity is ISoftDeletableEntity deletableEntity)
                    {
                        deletableEntity.MarkAsDeleted(userSession);
                        entry.State = EntityState.Modified;
                    }
                    break;
                case EntityState.Detached:
                case EntityState.Unchanged:
                    //do nothing
                    break;
                default:
                    throw new InvalidOperationException($"Invalid state {entry.State} for entity {entry.Entity.GetType().Name}.");
            }
        }
    }

    private void ValidateEntityChange(
        EntityBase entity,
        IUserSession userSession,
        EntityChangeState state)
    {
        _entityAuthorizationService.ValidateEntityChange(entity, userSession, state);
    }
}
