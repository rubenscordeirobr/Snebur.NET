namespace Snebur.UseCases.Identities.Shared;

public sealed record UserSessionResponse : IUserSession, IResponse
{
    public required Guid Id { get; init; }
    public required string ApplicationName { get; init; }
    public required string IpAddress { get; init; }
    public required string UserAgent { get; init; }
    public required bool IsActive { get; init; }
    public required bool IsPersistent { get; init; }
    public required DateTime StartedAt { get; init; }
    public required DateTime? TerminatedAt { get; init; }
    public required DateTime LastActivity { get; init; }
    public required Language Language { get; init; }
    public required AuthenticationType AuthenticationType { get; init; }
    public required SessionTerminationReason? TerminationReason { get; init; }
    public required UserRole UserRole { get; init; }
    public required UserType UserType { get; init; }
    public required GeoLocation? GeoLocation { get; init; }
    public required Guid? Tenant_Id { get; init; }
    public required Guid User_Id { get; init; }

}
