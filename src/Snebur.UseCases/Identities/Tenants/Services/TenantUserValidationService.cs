namespace Snebur.UseCases.Identities.Tenants.Services;

internal class TenantUserValidationService : ITenantUserValidationService
{
    private readonly ITenantUserRepository _tenantUserRepository;

    public TenantUserValidationService(
        ITenantUserRepository tenantUserRepository)
    {
        _tenantUserRepository = tenantUserRepository;
    }

    public async Task<bool> IsEmailUniqueAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        email = SanitizeUtils.SanitizeEmail(email);

        return !await _tenantUserRepository.EmailExistsAsync(email, cancellationToken);
    }

    public async Task<bool> IsEmailUniqueAsync(
        Guid currentUser_Id,
        string email,
        CancellationToken cancellationToken = default)
    {
        email = SanitizeUtils.SanitizeEmail(email);

        return !await _tenantUserRepository.EmailExistsAsync(email, currentUser_Id, cancellationToken);
    }

    public async Task<bool> IsPhoneNumberUniqueAsync(
        string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        phoneNumber = SanitizeUtils.SanitizePhoneNumber(phoneNumber);

        return !await _tenantUserRepository.PhoneNumberExistsAsync(phoneNumber, cancellationToken);
    }

    public async Task<bool> IsPhoneNumberUniqueAsync(
        Guid currentUser_Id,
        string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        phoneNumber = SanitizeUtils.SanitizePhoneNumber(phoneNumber);

        return !await _tenantUserRepository.PhoneNumberExistsAsync(phoneNumber, currentUser_Id, cancellationToken);
    }
}
