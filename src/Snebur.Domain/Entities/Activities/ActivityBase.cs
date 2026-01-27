namespace Snebur.Domain.Entities.Activities;

public abstract record ActivityBase
{
    public string? Id { get; init; }
    public required Guid? Tenant_Id { get; init; }
    public required Guid UserSession_Id { get; init; }
    public required DateTime ActivityDate { get; init; }
    public required string? Description { get; init; }
    public abstract ActivityType ActivityType { get; }
}
