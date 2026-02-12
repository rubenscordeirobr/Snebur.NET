namespace Snebur.Domain.Entities.Activities;

public abstract record EntityActivity : ActivityBase
{
    public required string QualifiedTypeName { get; init; }
    public required Guid Entity_Id { get; init; }
}
