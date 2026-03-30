using System.Text.Json.Serialization;
using Snebur.SharedKernel.Interfaces.Identities;

namespace Snebur.SharedKernel.Models.Identities;

public class CachedUser : IUser
{
    public Guid Id { get; }
    public string Name { get; }
    public string Email { get; }
    public VerificationState EmailVerificationState { get; }
    public VerificationState PhoneNumberVerificationState { get; }
    public PhoneNumber PhoneNumber { get; }
    public Language Language { get; }
    public UserRole Role { get; }
    public UserType UserType { get; }

    [JsonConstructor]
    private CachedUser(
        Guid id,
        string name,
        string email,
        VerificationState emailVerificationState,
        VerificationState phoneNumberVerificationState,
        PhoneNumber phoneNumber,
        Language language,
        UserRole role,
        UserType userType
    )
    {
        Id = id;
        Name = name;
        Email = email;
        EmailVerificationState = emailVerificationState;
        PhoneNumberVerificationState = phoneNumberVerificationState;
        PhoneNumber = phoneNumber;
        Language = language;
        Role = role;
        UserType = userType;
    }

    public static CachedUser Create(IUser user)
    {
        Guard.NotNull(user);

        return new CachedUser(
            user.Id,
            user.Name,
            user.Email,
            user.EmailVerificationState,
            user.PhoneNumberVerificationState,
            user.PhoneNumber,
            user.Language,
            user.Role,
            user.UserType
         );
    }

    internal static CachedUser Create(
        Guid id,
        string name,
        string email,
        VerificationState emailVerificationState,
        VerificationState phoneNumberVerificationState,
        PhoneNumber phoneNumber,
        Language language,
        UserRole role,
        UserType userType)
    {
        return new CachedUser(
            id,
            name,
            email,
            emailVerificationState,
            phoneNumberVerificationState,
            phoneNumber,
            language,
            role,
            userType
        );
    }

}
