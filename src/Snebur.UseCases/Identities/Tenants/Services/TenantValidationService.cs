namespace Snebur.UseCases.Identities.Tenants.Services;

internal class TenantValidationService : ITenantValidationService
{
    private readonly ITenantRepository _tenantRepository;
    private readonly ITenantUserRepository _tenantUserRepository;

    public TenantValidationService(
        ITenantRepository tenantRepository,
        ITenantUserRepository tenantUserRepository)
    {
        _tenantRepository = tenantRepository;
        _tenantUserRepository = tenantUserRepository;
    }

    public async Task<bool> IsEmailUniqueAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        email = SanitizeUtils.SanitizeEmail(email);

        return !await _tenantRepository.EmailExistsAsync(email, cancellationToken) &&
            !await _tenantUserRepository.EmailExistsAsync(email, cancellationToken);
    }

    public async Task<bool> IsEmailUniqueAsync(
        Guid currentTenant_Id,
        Guid currentTenantOwner_Id,
        string email,
        CancellationToken cancellationToken = default)
    {
        email = SanitizeUtils.SanitizeEmail(email);

        return !await _tenantRepository.EmailExistsAsync(email, currentTenant_Id, cancellationToken)
            && !await _tenantUserRepository.EmailExistsAsync(email, currentTenantOwner_Id, cancellationToken);
    }

    public async Task<bool> IsFiscalCodeUniqueAsync(
        string fiscalCode,
        CancellationToken cancellationToken = default)
    {
        fiscalCode = SanitizeUtils.SanitizeFiscalCode(fiscalCode);
        return !await _tenantRepository
            .FiscalCodeExistsAsync(fiscalCode, cancellationToken);
    }

    public async Task<bool> IsFiscalCodeUniqueAsync(
        Guid currentTenant_Id,
        string fiscalCode,
        CancellationToken cancellationToken = default)
    {
        fiscalCode = SanitizeUtils.SanitizeFiscalCode(fiscalCode);

        return !await _tenantRepository
            .FiscalCodeExistsAsync(fiscalCode, currentTenant_Id, cancellationToken);
    }

    public async Task<bool> IsPhoneNumberUniqueAsync(
        string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        phoneNumber = SanitizeUtils.SanitizePhoneNumber(phoneNumber);

        return !await _tenantRepository.PhoneNumberExitsAsync(phoneNumber, cancellationToken) &&
            !await _tenantUserRepository.PhoneNumberExistsAsync(phoneNumber, cancellationToken);
    }

    public async Task<bool> IsPhoneNumberUniqueAsync(
        Guid currentTenant_Id,
        Guid currentTenantOwner_Id,
        string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        phoneNumber = SanitizeUtils.SanitizePhoneNumber(phoneNumber);

        return !await _tenantRepository.PhoneNumberExitsAsync(phoneNumber, currentTenant_Id, cancellationToken)
            && !await _tenantUserRepository.PhoneNumberExistsAsync(phoneNumber, currentTenantOwner_Id, cancellationToken);
    }
}
