using System.Linq.Expressions;

namespace Snebur.Application.Abstractions.Persistence;
public interface IRepositoryBase<TEntity> where TEntity : EntityBase
{
    //queries
    Task<TEntity?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object?>>[] includeExpressions);

    Task<TEntity?> FindAsync(
           Expression<Func<TEntity, bool>> filterExpression,
           CancellationToken cancellationToken = default,
           params Expression<Func<TEntity, object?>>[] includeExpressions);

    Task<IEnumerable<TEntity>> FindAllAsync(
        Expression<Func<TEntity, bool>> filterExpression,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object?>>[] includeExpressions);

    Task<IEnumerable<TEntity>> GetAllAsync(
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object?>>[] includeExpressions);
     
    Task<TEntity> RefreshAsync(
        TEntity entity,
        CancellationToken cancellationToken = default);

    Task<TEntity?> TryRefreshAsync(
        TEntity entity,
        CancellationToken cancellationToken = default);

    //validations
    Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> filterExpression,
        CancellationToken cancellationToken = default);

    IRepositoryBase<TEntity> NoTracking();

    IRepositoryBase<TEntity> Tracking();
}
