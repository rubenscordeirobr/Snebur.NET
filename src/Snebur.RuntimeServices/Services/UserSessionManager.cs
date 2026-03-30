using Snebur.SharedKernel.Factories;
using Snebur.SharedKernel.Models.Security;
using Microsoft.Extensions.Logging;

namespace Snebur.RuntimeServices.Services;

public sealed class UserSessionManager : IUserSessionManager
{
    private readonly IHttpContextSessionAccessor _httpContextSessionAccessor;
    private readonly IUserSessionTokenHandler _userSessionTokenHandler;
    private readonly IUserSessionCacheService _userSessionCacheService;
    private readonly ILogger<UserSessionManager> _logger;

    public UserSessionManager(
        IHttpContextSessionAccessor httpContextSessionAccessor,
        IUserSessionTokenHandler userSessionTokenFactory,
        IUserSessionCacheService userSessionCacheService,
        ILogger<UserSessionManager> logger)
    {
        Guard.NotNull(httpContextSessionAccessor);

        _httpContextSessionAccessor = httpContextSessionAccessor;
        _userSessionTokenHandler = userSessionTokenFactory;
        _userSessionCacheService = userSessionCacheService;
        _logger = logger;
        _httpContextSessionAccessor.UserSessionClaims = InitializeUserSessionClaims();
    }

    public Guid? UserSession_Id
        => _httpContextSessionAccessor.UserSession_Id;

    public async Task<IUserSession?> GetSessionAsync()
    {
        var userSession = _httpContextSessionAccessor.UserSession;
        if (userSession is not null)
        {
            return userSession;
        }

        if (UserSession_Id is null)
        {
            return null;
        }

        return await _userSessionCacheService.GetSessionAsync(UserSession_Id.Value);
    }

    public async Task SetSessionAsync(IUserSession userSession, IUser user )
    {
        try
        {
            Guard.NotNull(userSession);

            var claims = UserSessionClaimsFactory.Create(userSession, user);
            var authorizationToken = _userSessionTokenHandler.WriteToken(claims, userSession.IsPersistent);

            _httpContextSessionAccessor.UserSession = userSession;
            _httpContextSessionAccessor.UserSessionClaims = claims;
            _httpContextSessionAccessor.AuthorizationToken = authorizationToken;

            await _userSessionCacheService.AddSessionAsync(userSession);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding client session.");
        }
    }
      
    public async Task RemoveSessionAsync()
    {
        if (UserSession_Id.HasValue)
        {
            await RemoveSessionAsync(UserSession_Id.Value);
        }
    }

    public async Task RemoveSessionAsync(Guid userSession_Id)
    {
        if (UserSession_Id == userSession_Id)
        {
            _httpContextSessionAccessor.AuthorizationToken = null;
        }
        await _userSessionCacheService.RemoveSessionAsync(userSession_Id);
    }

    private UserSessionClaims? InitializeUserSessionClaims()
    {
        var authorizationToken = _httpContextSessionAccessor.AuthorizationToken;
        if (string.IsNullOrWhiteSpace(authorizationToken))
        {
            return null;
        }

        try
        {
            return _userSessionTokenHandler.ReadToken(authorizationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading client session token. {ClientToken}", authorizationToken);
            return null;
        }
    }
}
