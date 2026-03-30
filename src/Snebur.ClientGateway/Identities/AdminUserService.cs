using Snebur.UseCases.Identities.Users.AdminUsers.Queries;

namespace Snebur.ClientGateway.Identities;

[Route(IdentityRouteConstants.AdminUsersRoute)]
public class AdminUserService : IAdminUserService
{
    private readonly IHttpClientMediator<AdminUserService> _mediator;

    public AdminUserService(IHttpClientMediator<AdminUserService> mediator)
    {
        _mediator = mediator;
    }

    #region Queries
    public Task<Result<UserResponse>> GetAdminUserByIdAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        var query = new GetAdminUserByIdQuery(id);
        return _mediator.GetAsync(
            query,
            RouteConstants.RouteId,
            cancellationToken);
    }

    public Task<Result<UserResponse>> GetAdminUserByEmailAsync(
        string email, 
        CancellationToken cancellationToken = default)
    {
        var query = new GetAdminUserByEmailQuery(email);
        return _mediator.GetAsync(
            query,
            cancellationToken);
    }

    public Task<Result<UserResponse>> GetAdminUserByPhoneNumberAsync(
        string phoneNumber, 
        CancellationToken cancellationToken = default)
    {
        var query = new GetAdminUserByPhoneNumberQuery(phoneNumber);
        return _mediator.GetAsync(
            query,
            cancellationToken);
    }

    public Task<Result<UserResponse>> GetAdminUserByEmailOrPhoneNumberAsync(
        string emailOrPhoneNumber,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAdminUserByEmailOrPhoneNumberQuery(emailOrPhoneNumber);
        return _mediator.GetAsync(
            query,
            cancellationToken);
    }

    #endregion
}
