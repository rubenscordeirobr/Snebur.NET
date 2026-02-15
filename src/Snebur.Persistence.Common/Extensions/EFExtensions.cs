using Snebur.Persistence.Common.Enums;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Snebur.Persistence.Common.Extensions;

public static class EFExtensions
{
    public static IQueryable<TEntity> ApplyTracking<TEntity>(
         this IQueryable<TEntity> dbSet,
         TrackingOption tracking)
         where TEntity : EntityBase
    {
        return tracking == TrackingOption.Tracking
            ? dbSet
            : dbSet.AsNoTracking();
    }

    public static bool HasChanges(this EntityEntry entry)
    {
        Guard.NotNull(entry);

        return entry.State switch
        {
            EntityState.Added => true,
            EntityState.Modified => true,
            EntityState.Deleted => true,
            _ => false
        };
    }

    public static bool IsInMemory(this DatabaseFacade database)
          => database?.ProviderName == "Microsoft.EntityFrameworkCore.InMemory";

}

