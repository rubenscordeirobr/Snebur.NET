using Snebur.Application.Exceptions;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Snebur.Persistence.Common.Interceptors;

public class DefaultSaveChangesInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        // Add your logic before saving changes
        throw new SyncSaveChangesNotAllowedException("Use SaveChangesAsync instead");
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        throw new SyncSaveChangesNotAllowedException("Use SaveChangesAsync instead");
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        Guard.NotNull(eventData);

        if (eventData.Context is not null)
        {
            var entries = eventData.Context.ChangeTracker.Entries<EntityBase>()
                .Where(entry => entry.State == EntityState.Added ||
                           entry.State == EntityState.Modified);

            foreach (var entity in entries.Select(entry => entry.Entity))
            {
                if (entity.CreatedSession_Id == Guid.Empty ||
                   entity.LastUpdatedSession_Id == Guid.Empty)
                {
                    throw new InvalidOperationException("" +
                        "The SaveChanges method must be call into the UnitOfWork class");
                }

                if (entity.Id != Guid.Empty &&
                    entity.LastUpdatedAt.AddSeconds(30) < DateTime.UtcNow)
                {
                    var message = $"The entity {entity.GetType().Name} {entity.Id} " +
                                  $" The last update must e less than 10 seconds";

                    throw new InvalidOperationException(message);
                }
            }
        }
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        //logic after saving changes asynchronously
        return base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    public override Task SaveChangesFailedAsync(
        DbContextErrorEventData eventData,
        CancellationToken cancellationToken = default)
    {
        //logic when saving changes fails asynchronously
        return base.SaveChangesFailedAsync(eventData, cancellationToken);
    }
}
