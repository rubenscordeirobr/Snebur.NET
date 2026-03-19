using System.Collections.Concurrent;

namespace Snebur.Domain.Helpers;

public static class EntityReflectionHelper
{
    private static readonly ConcurrentDictionary<Type, bool> _isSoftDeletableCache = new();
    private static readonly ConcurrentDictionary<Type, bool> _isTenantOwnedCache = new();

    public static bool IsImplementDeletedInterface<TEntity>()
        where TEntity : EntityBase
    {
        return IsImplementDeletedInterface(typeof(TEntity));
    }

    public static bool IsImplementDeletedInterface(Type entityType)
    {
        Guard.NotNull(entityType);
        return _isSoftDeletableCache.GetOrAdd(entityType, IsImplementDeletedInterfaceInternal);
    }

    public static bool IsImplementTenantOwnedInterface<TEntity>()
        where TEntity : EntityBase
    {
        return IsImplementTenantOwnedInterface(typeof(TEntity));
    }

    public static bool IsImplementTenantOwnedInterface(Type entityType)
    {
        Guard.NotNull(entityType);
        return _isTenantOwnedCache.GetOrAdd(entityType, IsImplementTenantOwnedInterfaceInternal);
    }

    private static bool IsImplementTenantOwnedInterfaceInternal(Type entityType)
    {
        return typeof(ITenantOwned).IsAssignableFrom(entityType);
    }

    private static bool IsImplementDeletedInterfaceInternal(Type entityType)
    {
        return typeof(ISoftDeletableEntity).IsAssignableFrom(entityType);
    }
}
