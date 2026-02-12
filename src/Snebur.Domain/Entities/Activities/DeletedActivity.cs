namespace Snebur.Domain.Entities.Activities;

public sealed record DeletedActivity : EntityActivity
{
    public required string DeletedData { get; init; }
    public sealed override ActivityType ActivityType
        => ActivityType.Deleted;
}
