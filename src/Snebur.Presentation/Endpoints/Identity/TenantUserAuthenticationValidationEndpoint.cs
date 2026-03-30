using Microsoft.AspNetCore.Authorization;

namespace Snebur.Presentation.Endpoints.Identity;

[AllowAnonymous]
[ServiceRole(ServiceRole.Authentication)]
[EndPoint(IdentityRouteConstants.TenantUserAuthenticationValidation)]
public class TenantUserAuthenticationValidationEndpoint : ApiEndpointBase, ITenantUserAuthenticationValidationService
{
    private readonly ITenantUserAuthenticationValidationService _validationService;
 
    public TenantUserAuthenticationValidationEndpoint(
        ITenantUserAuthenticationValidationService validationService)
    {
        _validationService = validationService;
    }

    [HttpForm]
    public Task<bool> VerifyTenantUserCredentialsAsync(
        string emailOrPhoneNumber,
        string password,
        CancellationToken cancellationToken = default)
    {
        return _validationService
            .VerifyTenantUserCredentialsAsync(emailOrPhoneNumber, password, cancellationToken);
    }

    [HttpForm]
    public Task<bool> EmailOrPhoneNumberExitsAsync(
        string emailOrPhoneNumber,
        CancellationToken cancellationToken = default)
    {
        return _validationService
            .EmailOrPhoneNumberExitsAsync(emailOrPhoneNumber, cancellationToken);
    }
}

