using Snebur.UseCases.Identities.Users.TenantUsers.Commands;
using Snebur.UseCases.Identities.Users.TenantUsers.Queries;

namespace Snebur.ClientGateway.Identities;

[Route(IdentityRouteConstants.TenantUsersRoute)]
public class TenantUserService : ITenantUserService
{
    private readonly IHttpClientMediator<TenantUserService> _mediator;

    public TenantUserService(IHttpClientMediator<TenantUserService> mediator)
    {
        _mediator = mediator;
    }

    #region Queries
    public Task<Result<UserResponse>> GetTenantUserByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetTenantUserByIdQuery(id);
        return _mediator.GetAsync(
            query,
            RouteConstants.RouteId,
            cancellationToken);
    }

    public Task<Result<UserResponse>> GetTenantUserByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var query = new GetTenantUserByEmailQuery(email);
        return _mediator.GetAsync(
            query,
            cancellationToken);
    }

    public Task<Result<UserResponse>> GetTenantUserByPhoneNumberAsync(
        string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        var query = new GetTenantUserByPhoneNumberQuery(phoneNumber);
        return _mediator.GetAsync(
            query,
            cancellationToken);

    }

    public Task<Result<UserResponse>> GetTenantUserByEmailOrPhoneNumberAsync(
        string emailOrPhoneNumber,
        CancellationToken cancellationToken = default)
    {
        var query = new GetTenantUserByEmailOrPhoneNumberQuery(emailOrPhoneNumber);
        return _mediator.GetAsync(
            query,
            cancellationToken);

    }

    #endregion

    #region Commands

    public Task<Result<CreateTenantUserResponse>> CreateTenantUserAsync(
        CreateTenantUserCommand command,
        CancellationToken cancellationToken = default)
    {
        return _mediator.PostAsync(
            command,
            cancellationToken);

    }
     
    public Task<Result<OperationResponse>> DeleteTenantUserAsync(
        DeleteTenantUserCommand command,
        CancellationToken cancellationToken = default)
    {
        return _mediator.DeleteAsync(
            command,
            cancellationToken);
    }

    #endregion
}
