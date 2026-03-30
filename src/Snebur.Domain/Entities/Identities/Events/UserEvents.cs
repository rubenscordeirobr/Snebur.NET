namespace Snebur.Domain.Entities.Identities.Events;

public record UserCreatedEvent(User UserCreated) : IDomainEvent;

public record PasswordChangedEvent(User User) : IDomainEvent;

public record UserLoggedInEvent (
    User User,
    AuthenticationType AuthenticationType,
    string UserIdentifier ,
    string IpAddress) : IDomainEvent;

public record UserLoginFailedEvent (
    User User,
    string UserIdentifier,
    string PasswordFailed,
    string IpAddress) : IDomainEvent;

public record UserLoggedOutEvent(
    User User, 
    string IpAddress) : IDomainEvent;

