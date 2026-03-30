using Microsoft.AspNetCore.Authorization;

namespace Snebur.Presentation.Endpoints.Identity;

[AllowAnonymous]
[ServiceRole(ServiceRole.Authentication)]
[EndPoint(IdentityRouteConstants.AdminUserAuthenticationValidation)]
public class AdminUserAuthenticationValidationEndpoint : ApiEndpointBase, IAdminUserAuthenticationValidationService
{
    private readonly IAdminUserAuthenticationValidationService _validationService;
 
    public AdminUserAuthenticationValidationEndpoint(
        IAdminUserAuthenticationValidationService validationService)
    {
        _validationService = validationService;
    }

    [HttpForm]
    public Task<bool> VerifyAdminUserCredentialsAsync(
        string emailOrPhoneNumber,
        string password,
        CancellationToken cancellationToken = default)
    {
        return _validationService
            .VerifyAdminUserCredentialsAsync(emailOrPhoneNumber, password, cancellationToken);
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
