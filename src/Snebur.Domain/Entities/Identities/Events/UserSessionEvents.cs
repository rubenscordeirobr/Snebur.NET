namespace Snebur.Domain.Entities.Identities.Events;

public sealed record UserSessionStartedEvent(
        UserSession UserSession) : IDomainEvent;

public sealed record UserSessionTerminatedEvent(
        UserSession UserSession,
        SessionTerminationReason Reason) : IDomainEvent;

public sealed record UserSessionLanguageChangedEvent
        (UserSession UserSession, Language Language) : IDomainEvent;
