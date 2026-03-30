using Snebur.UseCases.Identities.Authentications.Commands;

namespace Snebur.ClientGateway.Identities;

[Route(IdentityRouteConstants.AdminUserAuthentication)]
public class AdminUserAuthenticationService : IAdminUserAuthenticationService
{
    private readonly IClientAuthorizationTokenManager _tokenAuthorizationTokenManager;
    private readonly IClientAdminUserSessionContextService _sessionContextService;
    private readonly IHttpClientMediator<AdminUserAuthenticationService> _mediator;

    public AdminUserAuthenticationService(
        IClientAuthorizationTokenManager clientAuthorizationTokenManager,
        IClientAdminUserSessionContextService sessionContextService,
        IHttpClientMediator<AdminUserAuthenticationService> mediator)
    {
        _mediator = mediator;
        _tokenAuthorizationTokenManager = clientAuthorizationTokenManager;
        _sessionContextService = sessionContextService;
    }

    public async Task<Result<AdminUserLoginResponse>> LoginAsync(
        AdminUserLoginCommand command,
        CancellationToken cancellationToken = default)
    {
        Guard.NotNull(command);

        var result = await _mediator.PostAsync(command, cancellationToken);
        if (result.IsSuccess)
        {
            var response = result.Value;

            await _tokenAuthorizationTokenManager.SetAuthorizationTokenAsync(
                response.AuthorizationToken,
                command.IsPersistent);

            var claims = _tokenAuthorizationTokenManager.GetUserSessionClaims();
            if (claims is null)
            {
                return Result.Failure<AdminUserLoginResponse>(
                    new AuthenticationError(
                        "IClientAuthorizationTokenManager.UserSessionClaimsInvalid",
                        "Failed to set authorization token"));
            }

            if (claims.UserType != UserType.AdminUser)
            {
                return Result.Failure<AdminUserLoginResponse>(
                    new AuthenticationError(
                        "IClientAuthorizationTokenManager.UserTypeInvalid",
                        "User type is invalid"));
            }

            var sessionContext = new AdminUserSessionContext(
                claims,
                response.UserSession,
                response.User);

            await _sessionContextService.SetSessionContextAsync(sessionContext, claims.IsPersistent);
                 
        }
        return result;
    }

    public async Task<Result<OperationResponse>> LogoutAsync(
        AdminUserLogoutCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.PostAsync(command,
            IdentityRouteConstants.Logout,
            cancellationToken);

        if (result.IsSuccess)
        {
            await _tokenAuthorizationTokenManager.RemoveAuthorizationTokenAsync();
            await _sessionContextService.ClearSessionContextAsync();
        }
        return result;
    }
}

