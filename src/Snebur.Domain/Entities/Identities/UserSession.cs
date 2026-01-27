namespace Snebur.Domain.Entities.Identities;

public sealed class UserSession : EntityBase, IUserSession, IEventAggregate
{
    private readonly List<IDomainEvent> _events = new();
    public string ApplicationName { get; private set; }
    public string IpAddress { get; private set; }
    public string UserAgent { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsPersistent { get; private set; }
    public DateTime LastActivity { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime? TerminatedAt { get; private set; }
    public Language Language { get; private set; }
    public AuthenticationType AuthenticationType { get; private set; }
    public SessionTerminationReason? TerminationReason { get; private set; }
    public UserRole UserRole { get; private set; }
    public UserType UserType { get; private set; }
    public Guid User_Id { get; private set; }
    public User? User { get; private set; }
    public Guid? Tenant_Id { get; private set; }
    public Tenant? Tenant { get; private set; }
    public GeoLocation GeoLocation { get; private set; } = GeoLocation.Empty;

    // EF Core
    internal UserSession(
        string applicationName,
        string ipAddress,
        string userAgent,
        bool isActive,
        bool isPersistent,
        AuthenticationType authenticationType,
        Language language,
        UserRole userRole,
        UserType userType,
        Guid user_Id,
        Guid? tenant_Id)
    {
        ApplicationName = applicationName;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        IsPersistent = isPersistent;
        AuthenticationType = authenticationType;
        Language = language;
        UserRole = userRole;
        UserType = userType;
        User_Id = user_Id;
        Tenant_Id = tenant_Id;
        IsActive = isActive;
    }
      
    internal void AddSessionStartedEvents(IUser user)
    {
        _events.Add(new UserSessionStartedEvent(this));

        if (user.UserType == UserType.TenantUser)
        {
            _events.Add(new TenantUserSessionStartedEvent(this));
        }
    }
     
    public void TerminateSession(SessionTerminationReason reason)
    {
        if (Id == AnonymousUserConstants.Session_Id)
        {
            throw new InvalidOperationException("Anonymous system session cannot be terminated.");
        }

        if (!IsActive)
        {
            throw new InvalidOperationException("Session is already terminated.");
        }

        IsActive = false;
        TerminatedAt = DateTime.UtcNow;
        TerminationReason = reason;

        _events.Add(new UserSessionTerminatedEvent(this, reason));

        if (UserType == UserType.TenantUser)
        {
            _events.Add(new TenantUserSessionTerminatedEvent(this));
        }
    }

    public void UpdateLastActivity()
    {
        LastUpdatedAt = DateTime.UtcNow;
    }

    public void ChangeLanguage(Language language)
    {
        Language = language;
        _events.Add(new UserSessionLanguageChangedEvent(this, language));
    }
     
    #region IUser, IDomainEventAggregate

    public IReadOnlyList<IDomainEvent> DomainEvents => _events;

    #endregion

}
