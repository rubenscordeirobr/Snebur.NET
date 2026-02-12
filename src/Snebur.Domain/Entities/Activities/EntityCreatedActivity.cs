namespace Snebur.Domain.Entities.Activities;

public sealed record CreatedActivity : EntityActivity
{
    public required string CreatedData { get; init; }

    public sealed override ActivityType ActivityType
        => ActivityType.Created;

}
