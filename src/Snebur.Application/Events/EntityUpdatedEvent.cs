namespace Snebur.Application.Events;

public sealed record EntityUpdatedEvent<TEntity> 
    : EntityChangeStateEvent<TEntity>, IEntityUpdatedEvent<TEntity>
    where TEntity : EntityBase
{
    public override EntityChangeState State
        => EntityChangeState.Updated;

    public IReadOnlyList<IChangedPropertyEvent> ChangedProperties { get; }

    public EntityUpdatedEvent(
        TEntity entity,
        IReadOnlyList<IChangedPropertyEvent> changedProperties)
        : base(entity)
    {
        ChangedProperties = changedProperties;
    }
}

