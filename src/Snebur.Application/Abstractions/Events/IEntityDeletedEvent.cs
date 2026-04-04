namespace Snebur.Application.Abstractions.Events;

public interface IEntityDeletedEvent<TEntity> 
    : IEntityStateChangedEvent<TEntity>
    where TEntity : EntityBase
{
    IReadOnlyList<IPropertyValueEvent> PropertyValues { get; }

    EntityChangeState IEntityStateChangedEvent.State
        => EntityChangeState.Deleted;
}

