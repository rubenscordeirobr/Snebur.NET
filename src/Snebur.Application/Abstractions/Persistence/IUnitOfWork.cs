using Snebur.Application.Common;

namespace Snebur.Application.Abstractions.Persistence;

public interface IUnitOfWork : IAsyncDisposable
{
    //commands
    public void Add<TEntity>(TEntity entity)
        where TEntity : EntityBase;

    public void Update<TEntity>(TEntity entity)
        where TEntity : EntityBase;

    public void Delete<TEntity>(TEntity entity)
        where TEntity : EntityBase;

    public void Attach<TEntity>(TEntity entity)
        where TEntity : EntityBase;

    //transaction
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    Task<SaveChangesResult> CommitAsync(bool silent, CancellationToken cancellationToken = default);
    Task<SaveChangesResult> CommitAsync(CancellationToken cancellationToken = default);

    Task RollbackAsync(Exception exception, CancellationToken cancellationToken = default);

    Task<SaveChangesResult> SaveChangesAsync(bool silent, CancellationToken cancellationToken = default);
    Task<SaveChangesResult> SaveChangesAsync(CancellationToken cancellationToken = default);
}
