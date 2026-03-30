using Snebur.UseCases.Identities.Authentications.Commands;
using Snebur.UseCases.Identities.Tenants.Commands;

namespace Snebur.ClientGateway.Identities;

[Route(IdentityRouteConstants.TenantUserAuthentication)]
public class TenantUserAuthenticationService : ITenantUserAuthenticationService
{
    private readonly IClientAuthorizationTokenManager _tokenAuthorizationTokenManager;
    private readonly IClientTenantUserSessionContextService _sessionContextService;
    private readonly IHttpClientMediator<TenantUserAuthenticationService> _mediator;

    public TenantUserAuthenticationService(
        IClientAuthorizationTokenManager clientAuthorizationTokenManager,
        IClientTenantUserSessionContextService sessionContextService,
        IHttpClientMediator<TenantUserAuthenticationService> mediator)
    {
        _mediator = mediator;
        _tokenAuthorizationTokenManager = clientAuthorizationTokenManager;
        _sessionContextService = sessionContextService;
    }
     
    public async Task<Result<TenantUserLoginResponse>> LoginAsync(
        TenantUserLoginCommand command,
        CancellationToken cancellationToken = default)
    {
        Guard.NotNull(command);

        var result = await _mediator.PostAsync(command, 
            IdentityRouteConstants.Login, 
            cancellationToken);

        if (result.IsSuccess)
        {
            var response = result.Value;

            await _tokenAuthorizationTokenManager.SetAuthorizationTokenAsync(
                response.AuthorizationToken,
                command.IsPersistent);

            var userSessionClaims = _tokenAuthorizationTokenManager.GetUserSessionClaims();
            if (userSessionClaims is null)
            {
                return Result.Failure<TenantUserLoginResponse>(
                    new AuthenticationError(
                        "IClientAuthorizationTokenManager.UserSessionClaimsInvalid",
                        "Failed to set authorization token"));
            }

            if (userSessionClaims.UserType != UserType.TenantUser)
            {
                return Result.Failure<TenantUserLoginResponse>(
                    new AuthenticationError(
                        "IClientAuthorizationTokenManager.UserTypeInvalid",
                        "User type is invalid"));
            }

            var sessionContext = new TenantUserSessionContext(
                userSessionClaims,
                response.UserSession,
                response.User,
                response.Tenant);

            await _sessionContextService.SetSessionContextAsync(sessionContext, command.IsPersistent);
        }
        return result;
    }
     
    public async Task<Result<OperationResponse>> LogoutAsync(
        TenantUserLogoutCommand command,
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

    public async Task<Result<CreateTenantAccountResponse>> CreateTenantAccountAsync(
        CreateTenantAccountCommand command,
        CancellationToken cancellationToken = default)
    {
        Guard.NotEmpty(command);

        var result = await _mediator.PostAsync(
            command,
            IdentityRouteConstants.CreateTenantAccount,
            cancellationToken);

        if (result.IsSuccess && command.IsPersistent)
        {
            var loginCommand = new TenantUserLoginCommand
            {
                EmailOrPhoneNumber = command.Email,
                Password = command.Password,
                IsPersistent = command.IsPersistent
            };

            var loginResult = await LoginAsync(loginCommand,  cancellationToken);
            if (loginResult.IsFailure)
            {
                return Result.Failure<CreateTenantAccountResponse>(loginResult.Error);
            }
        }
        return result;
    }
}

