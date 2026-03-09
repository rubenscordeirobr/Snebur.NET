using Microsoft.AspNetCore.Authorization;

namespace Snebur.Presentation.Endpoints.Identity;

[AllowAnonymous]
[ServiceRole(ServiceRole.Validation)]
[EndPoint(IdentityRouteConstants.TenantValidation)]
public class TenantValidationEndpoint : ApiEndpointBase, ITenantValidationService
{
    private readonly ITenantValidationService _validationService;

    public TenantValidationEndpoint(
        ITenantValidationService tenantValidationService)
    {
        _validationService = tenantValidationService;
    }

    [HttpForm]
    public Task<bool> IsEmailUniqueAsync(
        string email, 
        CancellationToken cancellationToken = default)
    {
        return _validationService.IsEmailUniqueAsync(email, cancellationToken);
    }

    [HttpForm]
    public Task<bool> IsEmailUniqueAsync(
        Guid currentTenant_Id,
        Guid currentTenantOwner_Id,
        string email,
        CancellationToken cancellationToken = default)
    {
        return _validationService.IsEmailUniqueAsync(
            currentTenant_Id,
            currentTenantOwner_Id,
            email,
            cancellationToken);
    }

    [HttpForm]
    public Task<bool> IsFiscalCodeUniqueAsync(
        string fiscalCode, 
        CancellationToken cancellationToken = default)
    {
        return _validationService.IsFiscalCodeUniqueAsync(fiscalCode, cancellationToken);
    }

    [HttpForm]
    public Task<bool> IsFiscalCodeUniqueAsync(
        Guid currentTenant_Id,
        string fiscalCode,
        CancellationToken cancellationToken = default)
    {
        return _validationService.IsFiscalCodeUniqueAsync(
            currentTenant_Id,
            fiscalCode, cancellationToken);
    }

    [HttpForm]
    public Task<bool> IsPhoneNumberUniqueAsync(
        string phoneNumber, 
        CancellationToken cancellationToken = default)
    {
        return _validationService.IsPhoneNumberUniqueAsync(
            phoneNumber,
            cancellationToken);
    }

    [HttpForm]
    public Task<bool> IsPhoneNumberUniqueAsync(
        Guid currentTenant_Id, 
        Guid currentTenantOwner_Id,
        string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        return _validationService.IsPhoneNumberUniqueAsync(
            currentTenant_Id,
            currentTenantOwner_Id,
            phoneNumber,
            cancellationToken);
    }
 
}
