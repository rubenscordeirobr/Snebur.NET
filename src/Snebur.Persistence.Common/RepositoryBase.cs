using System.Linq.Expressions;
using Snebur.Domain.Helpers;
using Snebur.Persistence.Common.Enums;
using Snebur.Persistence.Common.Exceptions;

namespace Snebur.Persistence.Common;

public abstract class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : EntityBase
{
    private readonly DbContext _dbContext;
    private readonly IHttpContextSessionAccessor _userSessionAccessor;
    private TrackingOption _trackingOption;

    private readonly bool _isImplementDeletedInterface;
    private readonly bool _isImplementTenantOwnedInterface;

    private bool _isIncludeDeleted;

    protected virtual int DefaultMaxRecords
        => RepositoryConstants.DefaultMaxRecords;

    protected IUserSession UserSession
        => _userSessionAccessor.GetRequiredUserSession();

    protected IEndpointService? EndpointInstance
        => _userSessionAccessor.EndpointInstance;

    protected RepositoryBase(
        DbContext dbContext,
        IHttpContextSessionAccessor userSessionAccessor,
        TrackingOption trackingOption)
    {
        Guard.NotNull(dbContext);

        _dbContext = dbContext;
        _trackingOption = trackingOption;
        _userSessionAccessor = userSessionAccessor;

        _isImplementDeletedInterface = EntityReflectionHelper.IsImplementDeletedInterface<TEntity>();
        _isImplementTenantOwnedInterface = EntityReflectionHelper.IsImplementTenantOwnedInterface<TEntity>();
    }

    #region Queries
    public async Task<TEntity?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object?>>[] includeExpressions)
    {
        return await CreateQuery(includeExpressions)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<TEntity?> FindAsync(
        Expression<Func<TEntity, bool>> filterExpression,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object?>>[] includeExpressions)
    {
        Guard.NotNull(filterExpression);

        return await CreateQuery(includeExpressions)
            .FirstOrDefaultAsync(filterExpression, cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> FindAllAsync(
        Expression<Func<TEntity, bool>> filterExpression,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object?>>[] includeExpressions)
    {
        Guard.NotNull(filterExpression);

        return await CreateQuery(includeExpressions)
            .Where(filterExpression)
            .Take(DefaultMaxRecords)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object?>>[] includeExpressions)
    {
        return await CreateQuery(includeExpressions)
            .Take(DefaultMaxRecords)
            .ToListAsync(cancellationToken);
    }

    #endregion

    public async Task<TEntity> RefreshAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        Guard.NotNull(entity);

        var refreshedEntity = await TryRefreshAsync(entity, cancellationToken);

        return refreshedEntity is not null
            ? refreshedEntity
            : throw new EntityNotFoundException(typeof(TEntity), entity!.Id);
    }

    public async Task<TEntity?> TryRefreshAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        Guard.NotNull(entity);

        if (entity.Id == Guid.Empty)
            throw new ArgumentException("Is not possible to refresh entity with new entity.");

        var entry = _dbContext.Entry(entity);

        if (entry.State == EntityState.Detached &&
            _trackingOption == TrackingOption.Tracking)
        {
            RemoveAnyEntityTracked(entity);
            return await GetByIdAsync(entity.Id, cancellationToken);
        }

        await entry.ReloadAsync(cancellationToken);
        return entity;
    }

    #region validations

    public async Task<bool> ExistsAsync(
         Guid id,
         CancellationToken cancellationToken = default)
    {
        return await AnyAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> AnyAsync(
       Expression<Func<TEntity, bool>> filterExpression,
       CancellationToken cancellationToken = default)
    {
        Guard.NotNull(filterExpression);

        return await CreateQuery(null)
            .AnyAsync(filterExpression, cancellationToken);
    }
    #endregion

    private void RemoveAnyEntityTracked(TEntity entity)
    {
        var entry = _dbContext.ChangeTracker.Entries<TEntity>()
            .FirstOrDefault(x => x.Entity.Id == entity.Id);

        if (entry != null)
        {
            entry.State = EntityState.Detached;
        }
    }

    public void IncludeDeleted()
    {
        _isIncludeDeleted = true;
    }

    #region Filter

    protected virtual IQueryable<TEntity> CreateQuery(
        Expression<Func<TEntity, object?>>[]? includeExpressions)
    {
        // Warning: Never use Skip or Take before the filters.
        // If you don’t, EF Core will wrap your query in a subquery
        // It can hurt performance and lead to unexpected results.

        var query = _dbContext.Set<TEntity>()
              .AsQueryable()
              .ApplyTracking(_trackingOption);

        if (includeExpressions is not null && includeExpressions.Length > 0)
        {
            query = includeExpressions.Aggregate(query, (current, include) => current.Include(include));
        }

        if (_isImplementTenantOwnedInterface &&
            ShouldFilterTenantOwned())
        {
            query = query.Cast<ITenantOwned>()
               .Where(x => x.Tenant_Id == UserSession.Tenant_Id)
               .Cast<TEntity>();
        }

        if (_isImplementDeletedInterface && !_isIncludeDeleted)
        {
            query = query.Cast<ISoftDeletableEntity>()
                .Where(x => !x.IsDeleted)
                .Cast<TEntity>();
        }

        return query;
    }

    protected virtual bool ShouldFilterTenantOwned()
    {
        if (!UserSession.IsTenantUser() && !UserSession.IsSystemAdminUser())
        {
            throw new UnauthorizedAccessException("User not have permission to access this resource.");
        }
        return true;
    }

    public IRepositoryBase<TEntity> NoTracking()
    {
        _trackingOption = TrackingOption.NoTracking;
        return this;
    }

    public IRepositoryBase<TEntity> Tracking()
    {
        _trackingOption = TrackingOption.Tracking;
        return this;
    }

    #endregion
}
