using Snebur.SharedKernel.Models.Security;

namespace Snebur.ClientGateway.Abstractions;

public interface IClientAuthorizationTokenManager
{
    UserSessionClaims? GetUserSessionClaims();
    UserSessionState? GetUserSessionState();

    Task SetAuthorizationTokenAsync(string authorizationToken, bool isPersistent);
    Task ValidateAuthorizationHeaderAsync(string responseAuthorizationHeader);
    Task<string?> GetAuthorizationTokenAsync();
    Task EnsureUserSessionStateAsync();
    Task RemoveAuthorizationTokenAsync();

    event EventHandlerAsync<AuthorizationTokenUpdatedEventArgs>? UserSessionStateUpdated;
}
