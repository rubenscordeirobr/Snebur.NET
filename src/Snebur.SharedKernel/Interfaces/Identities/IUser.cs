namespace Snebur.SharedKernel.Interfaces.Identities;

public interface IUser
{
    Guid Id { get; }
    string Name { get; }
    string Email { get; }
    Language Language { get; }
    UserRole Role { get; }
    UserType UserType { get; }
    VerificationState EmailVerificationState { get; }
    VerificationState PhoneNumberVerificationState { get; }
    PhoneNumber PhoneNumber { get; }
}
