using Snebur.UseCases.Identities.Users.TenantUsers.Commands;
using Snebur.UseCases.Identities.Users.TenantUsers.Queries;

namespace Snebur.Presentation.Endpoints.Identity;

[EndPoint(IdentityRouteConstants.TenantUsersRoute)]
public class TenantUsersEndpoint : ApiEndpointBase, ITenantUserService
{
    private readonly IRequestMediator _mediator;

    public TenantUsersEndpoint(IRequestMediator mediator)
    {
        _mediator = mediator;
    }

    #region Queries

    [HttpGet(RouteConstants.RouteId)]
    public Task<Result<UserResponse>> GetTenantUserByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _mediator.GetAsync(new GetTenantUserByIdQuery(id), cancellationToken);
    }

    [HttpGet]
    public Task<Result<UserResponse>> GetTenantUserByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return _mediator.GetAsync(new GetTenantUserByEmailQuery(email), cancellationToken);
    }

    [HttpGet]
    public Task<Result<UserResponse>> GetTenantUserByPhoneNumberAsync(
        string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        return _mediator.GetAsync(new GetTenantUserByPhoneNumberQuery(phoneNumber), cancellationToken);
    }
      
    [HttpGet]
    public Task<Result<UserResponse>> GetTenantUserByEmailOrPhoneNumberAsync(
       string emailOrPhoneNumber,
       CancellationToken cancellationToken = default)
    {
        return _mediator.GetAsync(new GetTenantUserByEmailOrPhoneNumberQuery(emailOrPhoneNumber), cancellationToken);
    }

    #endregion

    #region Commands

    [HttpPost]
    public Task<Result<CreateTenantUserResponse>> CreateTenantUserAsync(
        CreateTenantUserCommand command,
        CancellationToken cancellationToken = default)
    { 
        return _mediator.RunAsync(command, cancellationToken);
    }
 
    [HttpDelete]
    public Task<Result<OperationResponse>> DeleteTenantUserAsync(
        DeleteTenantUserCommand command,
        CancellationToken cancellationToken = default)
    {
        return _mediator.RunAsync(command, cancellationToken);
    }

    #endregion
}

