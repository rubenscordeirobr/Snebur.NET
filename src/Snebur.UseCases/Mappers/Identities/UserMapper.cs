namespace Snebur.UseCases.Mappers.Identities;

internal static class UserMapper
{
    internal static UserResponse ToResponse(User user)
    {
        Guard.NotNull(user);

        return new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Language = user.Language,
            Role = user.Role,
            UserType = user.UserType,
            EmailVerificationState = user.EmailVerificationState,
            PhoneNumberVerificationState = user.PhoneNumberVerificationState,
            PhoneNumber = user.PhoneNumber
        };
    }
}
