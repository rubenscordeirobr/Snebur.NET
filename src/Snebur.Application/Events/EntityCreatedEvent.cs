namespace Snebur.Application.Events;

public sealed record EntityCreatedEvent<TEntity> 
    : EntityChangeStateEvent<TEntity>, IEntityCreatedEvent<TEntity>
    where TEntity : EntityBase
{
    public override EntityChangeState State
        => EntityChangeState.Created;

    public IReadOnlyList<IPropertyValueEvent> PropertyValues { get; }

    public EntityCreatedEvent(
        TEntity entity, 
        IReadOnlyList<IPropertyValueEvent> propertyValues  )
        : base(entity) 
    {
        PropertyValues = propertyValues;
    }
}
