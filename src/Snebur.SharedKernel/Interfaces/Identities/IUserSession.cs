namespace Snebur.SharedKernel.Interfaces.Identities;
public interface IUserSession
{
    Guid Id { get; }
    string ApplicationName { get; }
    string IpAddress { get; }
    string UserAgent { get; }
    bool IsActive { get; }
    bool IsPersistent { get; }
    DateTime StartedAt { get; }
    DateTime? TerminatedAt { get; }
    DateTime LastActivity { get; }
    Language Language { get; }
    AuthenticationType AuthenticationType { get; }
    SessionTerminationReason? TerminationReason { get; }
    UserRole UserRole { get; }
    UserType UserType { get; }
    GeoLocation? GeoLocation { get; }
    Guid? Tenant_Id { get; }
    Guid User_Id { get; }
}
