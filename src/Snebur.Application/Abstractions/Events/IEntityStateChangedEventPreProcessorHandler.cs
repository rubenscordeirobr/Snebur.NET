namespace Snebur.Application.Abstractions.Events;

public interface IEntityStateChangedEventPreProcessorHandler<in TEvent> : IPreProcessorHandler<TEvent>
    where TEvent : IEntityStateChangedEvent
{
}

public interface IEntityCreatedPreProcessorHandler<TEntity>
    : IEntityStateChangedEventPreProcessorHandler<IEntityCreatedEvent<TEntity>>
    where TEntity : EntityBase
{
}

public interface IEntityUpdatedEventPreProcessorHandler<TEntity>
    : IEntityStateChangedEventPreProcessorHandler<IEntityUpdatedEvent<TEntity>>
    where TEntity : EntityBase
{
}

public interface IEntityDeletedPreProcessorHandler<TEntity>
    : IEntityStateChangedEventPreProcessorHandler<IEntityDeletedEvent<TEntity>>
    where TEntity : EntityBase
{
}
