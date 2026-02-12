namespace Snebur.Domain.Entities.Activities;

public sealed record UpdatedActivity : EntityActivity
{
    public required string OldData { get; init; }
    public required string NewData { get; init; }

    public sealed override ActivityType ActivityType
        => ActivityType.Updated;
 
}
