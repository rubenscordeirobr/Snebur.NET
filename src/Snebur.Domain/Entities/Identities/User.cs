namespace Snebur.Domain.Entities.Identities;

public abstract class User : EntityBase, IUser, ISoftDeletableEntity, IAscendingSortable, IEventAggregate
{
    private readonly List<IDomainEvent> _events = [];
    private readonly List<UserSession> _sessions = new();
    public string Name { get; protected set; }
    public string Email { get; protected set; }
    public UserRole Role { get; protected set; }
    public Language Language { get; protected set; }
    public UserState UserState { get; protected set; }
    public UserStatus UserStatus { get; protected set; }
    public VerificationState EmailVerificationState { get; protected set; }
    public VerificationState PhoneNumberVerificationState { get; protected set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public Password Password { get; private set; }
    public IReadOnlyList<UserSession> Sessions => _sessions;

    public abstract UserType UserType { get; }

    protected User(
        string name,
        string email,
        Language language,
        UserRole role,
        UserState userState,
        UserStatus userStatus,
        VerificationState emailVerificationState,
        VerificationState phoneNumberVerificationState,
        PhoneNumber phoneNumber,
        Password password)
    {
        Guard.NotNullOrWhiteSpace(name);
        Guard.NotNullOrWhiteSpace(email);
        Guard.NotNull(phoneNumber);
         
        Name = name;
        Email = email;
        Language = language;
        Role = role;
        UserState = userState;
        UserStatus = userStatus;
        EmailVerificationState = emailVerificationState;
        PhoneNumberVerificationState = phoneNumberVerificationState;
        PhoneNumber = phoneNumber;
        Password = password;
    }

    public void ChangePassword(Password password)
    {
        Guard.NotNull(password);

        if (password.Strength < PasswordStrength.Medium)
            throw new DomainException("Password must be strong");

        Password = password;
        _events.Add(new PasswordChangedEvent(this));
    }

    #region IEntityDeleted, IOrderableEntity, IDomainEventAggregate
    public bool IsDeleted { get; protected set; }
    public DateTime? DeletedAt { get; protected set; }
    public Guid? DeletedSession_Id { get; protected set; }
    public double? SortOrder { get; protected set; }

    IReadOnlyList<IDomainEvent> IEventAggregate.DomainEvents
        => _events;

    #endregion

}
