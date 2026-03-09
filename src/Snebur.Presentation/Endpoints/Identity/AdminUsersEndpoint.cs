using Snebur.UseCases.Identities.Users.AdminUsers.Queries;

namespace Snebur.Presentation.Endpoints.Identity;

[EndPoint(IdentityRouteConstants.AdminUsersRoute)]
public class AdminUsersEndpoint : ApiEndpointBase, IAdminUserService
{
    private readonly IRequestMediator _mediator;
    public AdminUsersEndpoint(IRequestMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet(routeTemplate: RouteConstants.RouteId)]
    public Task<Result<UserResponse>> GetAdminUserByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAdminUserByIdQuery(id);
        return _mediator.GetAsync(
            query,
            cancellationToken);
    }

    [HttpGet]
    public Task<Result<UserResponse>> GetAdminUserByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAdminUserByEmailQuery(email);
        return _mediator.GetAsync(
            query,
            cancellationToken);
    }

    [HttpGet]
    public Task<Result<UserResponse>> GetAdminUserByPhoneNumberAsync(
        string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAdminUserByPhoneNumberQuery(phoneNumber);
        return _mediator.GetAsync(
            query,
            cancellationToken);
    }

    [HttpGet]
    public Task<Result<UserResponse>> GetAdminUserByEmailOrPhoneNumberAsync(
        string emailOrPhoneNumber, 
        CancellationToken cancellationToken = default)
    {
        var query = new GetAdminUserByEmailOrPhoneNumberQuery(emailOrPhoneNumber);
        return _mediator.GetAsync(
            query,
            cancellationToken);
    }
}

