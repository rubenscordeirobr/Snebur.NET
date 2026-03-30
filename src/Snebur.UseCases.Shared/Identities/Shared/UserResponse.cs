namespace Snebur.UseCases.Identities.Shared;

public sealed record UserResponse : IUser, IResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required Language Language { get; init; }
    public required UserRole Role { get; init; }
    public required UserType UserType { get; init; }
    public required VerificationState EmailVerificationState { get; init; }
    public required VerificationState PhoneNumberVerificationState { get; init; }
    public required PhoneNumber PhoneNumber { get; init; }
}
