using System.IdentityModel.Tokens.Jwt;
using Snebur.ClientGateway.Abstractions;
using Snebur.SharedKernel.Models.Security;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace Snebur.UI.Services;

public class ClientAuthorizationTokenManager : IClientAuthorizationTokenManager
{
    private const string AuthorizationTokenKey = "Snebur:AuthorizationToken";

    private readonly JwtSecurityTokenHandler _tokenHandler = new();
    private readonly IJSRuntime _jsRuntime;
    private readonly IStorageService _storageService;
    private readonly ILogger<ClientAuthorizationTokenManager> _logger;

    private UserSessionState? _currentUserSessionState;

    public event EventHandlerAsync<AuthorizationTokenUpdatedEventArgs>? UserSessionStateUpdated;

    public ClientAuthorizationTokenManager(
        IJSRuntime jsRuntime,
        IStorageService localStorageService,
        ILogger<ClientAuthorizationTokenManager> logger)
    {
        _storageService = localStorageService;
        _logger = logger;
        _jsRuntime = jsRuntime;
    }

    public UserSessionClaims? GetUserSessionClaims()
    {
        return _currentUserSessionState?.UserSessionClaims;
    }

    public UserSessionState? GetUserSessionState()
    {
        return _currentUserSessionState;
    }

    public async Task EnsureUserSessionStateAsync()
    {
        if (_currentUserSessionState is null)
        {
            var authorizationToken = await GetAuthorizationTokenAsync();
            if (authorizationToken is null)
            {
                return;
            }
            RefreshUserSessionState(authorizationToken);
        }
    }

    public async Task<string?> GetAuthorizationTokenAsync()
    {
        if (!_jsRuntime.IsJsRuntimeInitialized())
        {
            return null;
        }
        return await _storageService.GetItemAsync<string>(AuthorizationTokenKey);
    }

    public async Task SetAuthorizationTokenAsync(string authorizationToken, bool isPersistent)
    {
        Guard.NotNullOrWhiteSpace(authorizationToken);
        await _storageService.SetItemAsync(AuthorizationTokenKey, authorizationToken, isPersistent);
        RefreshUserSessionState(authorizationToken);
    }

    public async Task ValidateAuthorizationHeaderAsync(string? responseAuthorizationHeader)
    {
        if (!_jsRuntime.IsJsRuntimeInitialized())
        {
            return;
        }

        var authorizationToken = JwtUtils.ExtractTokenFromAuthorizationHeader(responseAuthorizationHeader);
        if (authorizationToken is null)
        {
            await RemoveAuthorizationTokenAsync();
            return;
        }
        await SetAuthorizationTokenAsync(authorizationToken, true);
    }

    public async Task RemoveAuthorizationTokenAsync()
    {
        await _storageService.RemoveItemAsync(AuthorizationTokenKey);
        RefreshUserSessionState(null);
    }

    private void RefreshUserSessionState(string? token)
    {
        if (ShouldRefreshUserSessionState(token))
        {
            var userSessionState = GetUserSessionStateFromToken(token);
            if (_currentUserSessionState != userSessionState)
            {
                _currentUserSessionState = userSessionState;

                var args = new AuthorizationTokenUpdatedEventArgs(_currentUserSessionState);
                UserSessionStateUpdated?.InvokeAsync(this, args);
            }
        }
    }

    private bool ShouldRefreshUserSessionState(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return _currentUserSessionState is not null;
        }
        return _currentUserSessionState?.AuthorizationToken != token;
    }

    private UserSessionState? GetUserSessionStateFromToken(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        var userSessionClaims = TryReadToken(token);
        if (userSessionClaims is null)
            return null;

        return new UserSessionState(userSessionClaims, token);
    }
    private UserSessionClaims? TryReadToken(string token)
    {
        try
        {
            return ReadToken(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading client session token. {ClientToken}", token);
            return null;
        }
    }

    private UserSessionClaims? ReadToken(string token)
    {
        var jwtToken = _tokenHandler.ReadJwtToken(token);
        var result = UserSessionClaimsFactory.Create(jwtToken.Claims, jwtToken.ValidTo);
        if (result.IsSuccess)
        {
            return result.Value;
        }

        _logger.LogError("Error reading user session token. {Token}. Code: {Code}, Message: {Message} ",
            token,
            result.Error.Code,
            result.Error.Message);

        return null;
    }
}
