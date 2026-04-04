namespace Snebur.Application.Abstractions.Events;
 
public interface IEntityUpdatedEvent<TEntity> : IEntityStateChangedEvent<TEntity>
    where TEntity : EntityBase
{
    IReadOnlyList<IChangedPropertyEvent> ChangedProperties { get; }

    EntityChangeState IEntityStateChangedEvent.State
        => EntityChangeState.Updated;
}
