using Snebur.UseCases.Identities.Authentications.Commands;
using Microsoft.AspNetCore.Authorization;

namespace Snebur.Presentation.Endpoints.Identity;

[ServiceRole(ServiceRole.Authentication)]
[EndPoint(IdentityRouteConstants.AdminUserAuthentication)]
public class AdminUserAuthenticationEndpoint : ApiEndpointBase, IAdminUserAuthenticationService
{
    private readonly IRequestMediator _mediator;
 
    public AdminUserAuthenticationEndpoint(IRequestMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [AllowAnonymous]
    public Task<Result<AdminUserLoginResponse>> LoginAsync(
        AdminUserLoginCommand command,
        CancellationToken cancellationToken = default)
    {
        return _mediator.RunAsync(command, cancellationToken);
    }

    [HttpPost(IdentityRouteConstants.Logout)]
    public Task<Result<OperationResponse>> LogoutAsync(
        AdminUserLogoutCommand command,
        CancellationToken cancellationToken = default)
    {
        return _mediator.RunAsync(command, cancellationToken);
    }

}

