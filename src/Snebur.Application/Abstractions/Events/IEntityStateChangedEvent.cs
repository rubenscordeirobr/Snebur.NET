namespace Snebur.Application.Abstractions.Events;

public interface IEntityStateChangedEvent : IDomainEvent
{
    EntityChangeState State { get; }

    EntityBase EntityBase { get; }
}

public interface IEntityStateChangedEvent<out TEntity> : IEntityStateChangedEvent
    where TEntity : EntityBase
{
    TEntity Entity { get; }

    EntityBase IEntityStateChangedEvent.EntityBase
        => Entity;
}

