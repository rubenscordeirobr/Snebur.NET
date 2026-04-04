namespace Snebur.UseCases.Abstractions.Identities;

public interface ITenantUserAuthenticationValidationService : IValidationService
{
    Task<bool> EmailOrPhoneNumberExitsAsync(
        string emailOrPhoneNumber, 
        CancellationToken cancellationToken = default);

    Task<bool> VerifyTenantUserCredentialsAsync(
        string emailOrPhoneNumber,
        string password,
        CancellationToken cancellationToken = default);
}
