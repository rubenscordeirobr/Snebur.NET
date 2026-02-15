using Snebur.Core.Helpers;

namespace Snebur.Persistence.Common.UnitOfWorks;

public abstract partial class UnitOfWork<TDbContext> : IUnitOfWork
    where TDbContext : DbContext
{
    protected TDbContext DbContext { get; }
    protected IHttpContextSessionAccessor UserSessionAccessor { get; }

    private readonly IEntityAuthorizationService _entityAuthorizationService;
    private readonly IEventMediator _eventMediator;
    private readonly ILogger _logger;

    private TransactionUnitOfWorkExecutor? _transactionExecutor;
    private readonly SemaphoreSlim _lock = new(1, 1);

    protected UnitOfWork(
        TDbContext dbContext,
        IHttpContextSessionAccessor userSessionService,
        IEntityAuthorizationService entityAuthorizationService,
        IEventMediator eventMediator,
        ILogger logger)
    {
        Guard.NotNull(dbContext);
        Guard.NotNull(userSessionService);

        DbContext = dbContext;
        UserSessionAccessor = userSessionService;

        _entityAuthorizationService = entityAuthorizationService;
        _eventMediator = eventMediator;
        _logger = logger;

        if (dbContext.ChangeTracker.HasChanges())
        {
            throw new InvalidOperationException("There are tracked entities. Use SaveChangesAsync for this operation.");
        }
    }

    #region Commands

    public void Add<TEntity>(TEntity entity) where TEntity : EntityBase
    {
        Guard.NotNull(entity);

        if (entity.Id != Guid.Empty &&
            !entity.Id.IsZeroPrefixedGuid())
        {
            throw new InvalidOperationException("Entity already has an id.");
        }
        DbContext.Set<TEntity>().Add(entity);
    }

    public void Update<TEntity>(TEntity entity) where TEntity : EntityBase
    {
        VerifyEntityIsTracked(entity);
        DbContext.Set<TEntity>().Update(entity);
    }

    public void Delete<TEntity>(TEntity entity) where TEntity : EntityBase
    {
        VerifyEntityIsTracked(entity);
        DbContext.Set<TEntity>().Remove(entity);
    }

    public void Attach<TEntity>(TEntity entity) where TEntity : EntityBase
    {
        var entry = DbContext.Entry(entity);
        if (entry.State == EntityState.Detached)
        {
            DbContext.Set<TEntity>().Attach(entity);
        }
    }

    protected T LazyInitialize<T>(ref T? repository, Func<T> factory) where T : class
    {
        Guard.NotNull(factory);

        return repository ??= factory();
    }

    #endregion

    public async Task<SaveChangesResult> SaveChangesAsync(
       CancellationToken cancellationToken = default)
    {
        return await SaveChangesAsync(silent: false, cancellationToken);
    }

    public async Task<SaveChangesResult> SaveChangesAsync(
        bool silent,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _lock.WaitAsync(cancellationToken);

            if (_transactionExecutor != null)
            {
                return await _transactionExecutor.SaveChangesAsync(silent, cancellationToken);
            }

            var executor = new UnitOfWorkExecutor(
                DbContext,
                UserSessionAccessor,
                _entityAuthorizationService,
                _eventMediator,
                _logger);

            return await executor.SaveChangesAsync(silent, cancellationToken);
        }
        finally
        {
            _lock.Release();
        }
    }

    #region Transaction

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (DbContext.Database.IsInMemory())
            return;

        if (_transactionExecutor != null)
            throw new InvalidOperationException("Failed to begin transaction. There is already an open transaction.");

        try
        {
            await _lock.WaitAsync(cancellationToken);

            var transaction = await DbContext.Database.BeginTransactionAsync(cancellationToken);

            _transactionExecutor = new TransactionUnitOfWorkExecutor(
                DbContext,
                UserSessionAccessor,
                _entityAuthorizationService,
                _eventMediator,
                _logger,
                transaction);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<SaveChangesResult> CommitAsync(
        CancellationToken cancellationToken = default)
    {
        return await CommitAsync(silent: false, cancellationToken);
    }

    public async Task<SaveChangesResult> CommitAsync(
        bool silent,
        CancellationToken cancellationToken = default)
    {
        if (DbContext.Database.IsInMemory())
            return await SaveChangesAsync(silent, cancellationToken);

        if (_transactionExecutor == null)
            throw new InvalidOperationException("There is no active transaction to commit.");

        try
        {
            await _lock.WaitAsync(cancellationToken);

            var result = await _transactionExecutor.CommitAsync(silent, cancellationToken);
            _transactionExecutor = null;
            return result;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task RollbackAsync(
        Exception exception,
        CancellationToken cancellationToken = default)
    {

        if (DbContext.Database.IsInMemory())
            return;

        if (_transactionExecutor == null)
            throw new InvalidOperationException("Failed to rollback transaction. There is no open transaction.");

        try
        {
            await _lock.WaitAsync(cancellationToken);

            await _transactionExecutor.TryRollbackAsync(exception, cancellationToken);
            _transactionExecutor = null;
        }
        finally
        {
            _lock.Release();
        }
    }

    #endregion

    public virtual async ValueTask DisposeAsync()
    {
        _lock.Dispose();

        if (_transactionExecutor is not null)
        {
            try
            {
                await _transactionExecutor.DisposeAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during transaction dispose.");
            }

            _transactionExecutor = null;
        }
        GC.SuppressFinalize(this);
    }

    private void VerifyEntityIsTracked<TEntity>(TEntity entity) where TEntity : EntityBase
    {
        var entry = DbContext.Entry(entity);
        if (entry.State == EntityState.Detached)
        {
            throw new InvalidOperationException($"Entity {entity} is not tracked. Make sure to get entity from DbContext with the tracking option enabled.");
        }
    }
}
