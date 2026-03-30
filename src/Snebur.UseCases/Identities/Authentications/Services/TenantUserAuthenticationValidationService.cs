namespace Snebur.UseCases.Identities.Authentications.Services;

internal class TenantUserAuthenticationValidationService : ITenantUserAuthenticationValidationService
{
    private readonly ITenantUserRepository _tenantUserRepository;
    private readonly ISecureConfiguration _secureConfiguration;
     
    public TenantUserAuthenticationValidationService(
        ITenantUserRepository tenantRepository,
        ISecureConfiguration secureConfiguration)
    {
        _tenantUserRepository = tenantRepository;
        _secureConfiguration = secureConfiguration;
    }

    public async Task<bool> VerifyTenantUserCredentialsAsync(
        string emailOrPhoneNumber,
        string password, 
        CancellationToken cancellationToken = default)
    {
        emailOrPhoneNumber = SanitizeUtils.SanitizeEmailOrPhoneNumber(emailOrPhoneNumber);
       
        var tenantUser = await _tenantUserRepository.GetByEmailOrPhoneNumberAsync(emailOrPhoneNumber, cancellationToken);
        if (tenantUser == null)
        {
            return false;
        }

        var salt = _secureConfiguration.GetPasswordSalt();

        return PasswordHelper.VerifyPassword(
            password,
            tenantUser.Password.HashValue,
            salt);
    }

    public Task<bool> EmailOrPhoneNumberExitsAsync(
        string emailOrPhoneNumber,
        CancellationToken cancellationToken = default)
    {
        emailOrPhoneNumber = SanitizeUtils.SanitizeEmailOrPhoneNumber(emailOrPhoneNumber);

        return _tenantUserRepository.EmailOrPhoneNumberExistsAsync(emailOrPhoneNumber, cancellationToken);
    }
     
}
