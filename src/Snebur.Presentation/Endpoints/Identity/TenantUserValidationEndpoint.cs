
using Microsoft.AspNetCore.Authorization;

namespace Snebur.Presentation.Endpoints.Identity;

[AllowAnonymous]
[ServiceRole(ServiceRole.Authentication)]
[EndPoint(IdentityRouteConstants.TenantUserValidation)]
public class TenantUserValidationEndpoint : ApiEndpointBase, ITenantUserValidationService
{
    private readonly ITenantUserValidationService _validationService;
     
    public TenantUserValidationEndpoint(ITenantUserValidationService validationService)
    {
        _validationService = validationService;
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
        Guid currentUser_Id,
        string email,
        CancellationToken cancellationToken = default)
    {
        return _validationService.IsEmailUniqueAsync(currentUser_Id, email, cancellationToken);
    }

    [HttpForm]
    public Task<bool> IsPhoneNumberUniqueAsync(
        string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        return _validationService.IsPhoneNumberUniqueAsync(phoneNumber, cancellationToken);
    }

    [HttpForm]
    public Task<bool> IsPhoneNumberUniqueAsync(
        Guid currentUser_Id,
        string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        return _validationService.IsPhoneNumberUniqueAsync(currentUser_Id, phoneNumber, cancellationToken);
    }

    #region IEndpointService
 
  
    #endregion
}
