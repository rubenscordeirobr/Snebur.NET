namespace Snebur.UseCases.Identities.Authentications.Services;

internal class AdminUserAuthenticationValidationService : IAdminUserAuthenticationValidationService
{
    private readonly IAdminUserRepository _adminUserRepository;
    private readonly ISecureConfiguration _secureConfiguration;

    public AdminUserAuthenticationValidationService(
        IAdminUserRepository adminUserRepository,
        ISecureConfiguration secureConfiguration)
    {
        _adminUserRepository = adminUserRepository;
        _secureConfiguration = secureConfiguration;
    }

    public async Task<bool> VerifyAdminUserCredentialsAsync(
        string emailOrPhoneNumber,
        string password,
        CancellationToken cancellationToken = default)
    {
        emailOrPhoneNumber = SanitizeUtils.SanitizeEmailOrPhoneNumber(emailOrPhoneNumber);

        var user = await _adminUserRepository.GetByEmailOrPhoneNumberAsync(emailOrPhoneNumber, cancellationToken);
        if (user == null)
        {
            return false;
        }

        var salt = _secureConfiguration.GetPasswordSalt();

        return PasswordHelper.VerifyPassword(
            password,
            user.Password.HashValue,
            salt);
    }

    public Task<bool> EmailOrPhoneNumberExitsAsync(
        string emailOrPhoneNumber,
        CancellationToken cancellationToken = default)
    {
        emailOrPhoneNumber = SanitizeUtils.SanitizeEmailOrPhoneNumber(emailOrPhoneNumber);

        return _adminUserRepository.EmailOrPhoneNumberExistsAsync(emailOrPhoneNumber, cancellationToken);
    }
}
