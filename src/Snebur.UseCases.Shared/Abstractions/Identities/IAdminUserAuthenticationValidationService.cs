namespace Snebur.UseCases.Abstractions.Identities;

public interface IAdminUserAuthenticationValidationService : IValidationService
{
    Task<bool> EmailOrPhoneNumberExitsAsync(
        string emailOrPhoneNumber,
        CancellationToken cancellationToken = default);

    Task<bool> VerifyAdminUserCredentialsAsync(
        string emailOrPhoneNumber,
        string password,
        CancellationToken cancellationToken = default);
}
