namespace Snebur.Persistence.Common.Exceptions;
internal class EntityNotFoundException : EntityException
{
    public Guid Entity_Id { get; }
    public EntityNotFoundException(Type entityType, Guid id) 
        : base($"{entityType.Name} with id {id} not found.")
    {
        Entity_Id = id;
    }
 
}
