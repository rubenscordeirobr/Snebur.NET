namespace Snebur.Application.Events;

public sealed record EntityDeletedEvent<TEntity>
    : EntityChangeStateEvent<TEntity>, IEntityDeletedEvent<TEntity>
    where TEntity : EntityBase
{
    public sealed override EntityChangeState State
        => EntityChangeState.Deleted;

    public IReadOnlyList<IPropertyValueEvent> PropertyValues { get; }
   
    public EntityDeletedEvent(
        TEntity entity,
        IReadOnlyList<IPropertyValueEvent> propertyValues)
        : base(entity)
    {
        PropertyValues = propertyValues;
    }
 
}
