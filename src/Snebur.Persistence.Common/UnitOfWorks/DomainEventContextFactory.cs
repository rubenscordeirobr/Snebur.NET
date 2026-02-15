using Snebur.Application.Abstractions.Events;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Snebur.Persistence.Common.UnitOfWorks;

internal static class DomainEventContextFactory
{
    internal static DomainEventContext Create(
        IUserSession userSession,
        IEnumerable<EntityEntry<EntityBase>> entries)
    {
        Guard.NotNull(userSession);
        Guard.NotNull(entries);

        var domainEvents = new List<IDomainEvent>();

        foreach (var entry in entries)
        {
            if (entry is null) continue;

            if (entry.Entity is IEventAggregate aggregate && aggregate.DomainEvents?.Any() == true)
            {
                domainEvents.AddRange(aggregate.DomainEvents);
            }

            var changeEvent = CreateEntityStateChangedEven(entry);
            if (changeEvent is not null)
            {
                domainEvents.Add(changeEvent);
            }
        }

        return new DomainEventContext(userSession, domainEvents);
    }

    private static IEntityStateChangedEvent? CreateEntityStateChangedEven(
        EntityEntry<EntityBase> entry)
    {
        return entry.State switch
        {
            EntityState.Added => CreateEvent<IPropertyValueEvent>(entry, typeof(EntityCreatedEvent<>)),
            EntityState.Deleted => CreateEvent<IPropertyValueEvent>(entry, typeof(EntityDeletedEvent<>)),
            EntityState.Modified => CreateEvent<IChangedPropertyEvent>(entry, typeof(EntityUpdatedEvent<>)),
            _ => throw new InvalidOperationException("Failed to create entity state changed event. Invalid entity state.")
        };
    }

    private static IEntityStateChangedEvent CreateEvent<TPropertyEvent>(
        EntityEntry<EntityBase> entry,
        Type entityStateEventType)
        where TPropertyEvent : IPropertyEvent
    {
        
        var entity = entry.Entity;
        var entityType = entity.GetType();
        var eventType = entityStateEventType.MakeGenericType(entityType);
        var propertyEvents = GetPropertyEvents<TPropertyEvent>(entry);
        var parameters = new object[] { entity, propertyEvents };

        var entityChangedEvent = Activator.CreateInstance(eventType, parameters)
            ?? throw new InvalidOperationException("Failed to create entity state changed event.");

        if (entityChangedEvent is IEntityStateChangedEvent entityStateChangedEvent)
            return entityStateChangedEvent;

        throw new InvalidOperationException("Failed to create entity state changed event. Invalid event type.");
    }

    private static List<TPropertyEvent> GetPropertyEvents<TPropertyEvent>(
        EntityEntry<EntityBase> entry)
        where TPropertyEvent : IPropertyEvent
    {
        var propertyEvents = new List<TPropertyEvent>();
        foreach (var property in entry.Properties)
        {
            var propertyEvent = CreatePropertyEvent(property, entry.State);
            if (propertyEvent is not null)
            {
                propertyEvents.Add((TPropertyEvent)propertyEvent);
            }
        }
        return propertyEvents;
    }

    private static IPropertyEvent? CreatePropertyEvent(
        PropertyEntry property,
        EntityState state)
    {
        return state switch
        {
            EntityState.Modified => CreateChangedPropertyEvent(property),
            EntityState.Added or EntityState.Detached => CreatePropertyValueEvent(property),
            _ => null
        };
    }

    private static ChangedPropertyEvent? CreateChangedPropertyEvent(PropertyEntry property)
    {
        if (!property.IsModified)
            return null;

        var originalValue = property.OriginalValue;
        var currentValue = property.CurrentValue;

        if (EqualityComparer<object>.Default.Equals(originalValue, currentValue))
            return null;

        return new ChangedPropertyEvent(
            PropertyName: property.Metadata.Name,
            PreviousValue: originalValue,
            Value: currentValue
         );
    }

    private static PropertyValueEvent CreatePropertyValueEvent(PropertyEntry property)
    {
        return new PropertyValueEvent(
            PropertyName: property.Metadata.Name,
            Value: property.CurrentValue
        );
    }
}
