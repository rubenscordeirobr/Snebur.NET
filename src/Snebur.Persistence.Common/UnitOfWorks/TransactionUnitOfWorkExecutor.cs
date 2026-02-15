using Microsoft.EntityFrameworkCore.Storage;

namespace Snebur.Persistence.Common.UnitOfWorks;

internal class TransactionUnitOfWorkExecutor : UnitOfWorkExecutorBase, IAsyncDisposable
{
    private readonly TransactionDomainEventManager _transactionDomainEventContext;

    private IDbContextTransaction? _transaction;
    private int _totalRowAffects;

    public TransactionUnitOfWorkExecutor(
        DbContext dbContext,
        IHttpContextSessionAccessor userSessionAccessor,
        IEntityAuthorizationService entityAuthorizationService,
        IEventMediator eventMediator,
        ILogger logger,
        IDbContextTransaction transaction)
        : base(dbContext, userSessionAccessor, entityAuthorizationService, eventMediator, logger)
    {
        _transaction = transaction;
        _transactionDomainEventContext = new(userSessionAccessor.GetRequiredUserSession());
    }

    public override async Task<SaveChangesResult> SaveChangesAsync(
        bool silent,
        CancellationToken cancellationToken)
    {
        var result = await ExecuteSaveChangesAsync(silent, cancellationToken);
        if (result.IsSuccess)
        {
            _totalRowAffects += result.AffectedRows;
            return result;
        }

        var executionException = result.Exception;
        await TryRollbackAsync(executionException, cancellationToken);

        if (!silent)
        {
            throw executionException;
        }
        return result;
    }

    protected override Task DispatchAsyncDomainEventAsync(
        DomainEventContext domainEventContext,
        int rowAffects)
    {
        _transactionDomainEventContext.Add(domainEventContext);
        return Task.CompletedTask;
    }

    protected override Task PreProcessorDispatchAsync(DomainEventContext domainEventContext)
    {
        _transactionDomainEventContext.Add(domainEventContext);
        return Task.CompletedTask;
    }

    internal async Task<SaveChangesResult> CommitAsync(
        bool silent,
        CancellationToken cancellationToken)
    {
        Guard.NotNull(_transaction);

        var domainEventContext = _transactionDomainEventContext.GetDomainEventContext();
        try
        {
            await EventMediator.PreProcessorDispatchAsync(domainEventContext, cancellationToken);

            if (domainEventContext.IsCanceled)
            {
                await TryRollbackAsync(domainEventContext.Exception, cancellationToken);
                return SaveChangesResult.DomainEventError(domainEventContext, domainEventContext.Error);
            }

            if (cancellationToken.IsCancellationRequested)
            {
                await TryRollbackAsync(new OperationCanceledException(cancellationToken), cancellationToken);
                return SaveChangesResult.OperationCanceledError(domainEventContext, cancellationToken);
            }

            var result = await ExecuteSaveChangesAsync(silent, cancellationToken);
            if (!result.IsSuccess)
            {
                await TryRollbackAsync(result.Exception, cancellationToken);
                return result;
            }

            await _transaction.CommitAsync(cancellationToken);

            //Cancellation token is not passed to the event mediator dispatch because we want to ensure
            //that all domain events are dispatched even if the operation is canceled after the transaction commit.
            await EventMediator.DispatchAsync(domainEventContext, CancellationToken.None);

            return SaveChangesResult.Success(domainEventContext, _totalRowAffects);
        }
        catch (OperationCanceledException ex)
        {
            Logger.LogError(ex, "Operation canceled during transaction commit.");

            await TryRollbackAsync(ex, cancellationToken);
            if (!silent)
            {
                throw;
            }
            return SaveChangesResult.OperationCanceledError(ex, domainEventContext);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during transaction commit.");
            await TryRollbackAsync(ex, cancellationToken);
            if (silent)
            {
                return SaveChangesResult.SaveChangesError(ex, domainEventContext);
            }
            throw;
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    internal async Task TryRollbackAsync(
        Exception exception,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (_transaction is null)
                return;

            Logger.LogError(exception, "Rollback transaction due to error.");
            await _transaction.RollbackAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during transaction rollback.");
        }
        finally
        {
            await TryDisposeAsync();
        }
    }

    private async Task TryDisposeAsync()
    {
        try
        {
            if (_transaction is null)
                return;

            await _transaction.DisposeAsync();
            _transaction = null;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during transaction dispose.");
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction is not null)
        {
            await TryRollbackAsync(new OperationCanceledException("The transaction executed was with opened transaction."), default);
        }
        _transaction = null;
    }
}
