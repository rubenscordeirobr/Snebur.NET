using Snebur.UseCases.Identities.Authentications.Commands;
using Snebur.UseCases.Identities.Tenants.Commands;
using Microsoft.AspNetCore.Authorization;

namespace Snebur.Presentation.Endpoints.Identity;

[ServiceRole(ServiceRole.Authentication)]
[EndPoint(IdentityRouteConstants.TenantUserAuthentication)]
public class TenantUserAuthenticationEndpoint : ApiEndpointBase, ITenantUserAuthenticationService
{
    private readonly IRequestMediator _mediator;
 
    public TenantUserAuthenticationEndpoint(IRequestMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost()]
    [AllowAnonymous]
    public Task<Result<TenantUserLoginResponse>> LoginAsync(
        TenantUserLoginCommand command,
        CancellationToken cancellationToken = default)
    {
        return _mediator.RunAsync(command, cancellationToken);
    }

    [HttpPost()]
    public Task<Result<OperationResponse>> LogoutAsync(
        TenantUserLogoutCommand command,
        CancellationToken cancellationToken = default)
    {
        return _mediator.RunAsync(command, cancellationToken);
    }

    [HttpPost()]
    [AllowAnonymous]
    public Task<Result<CreateTenantAccountResponse>> CreateTenantAccountAsync(
        CreateTenantAccountCommand command,
        CancellationToken cancellationToken = default)
    {
        return _mediator.RunAsync(command, cancellationToken);
    }

}

