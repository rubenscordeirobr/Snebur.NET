using Snebur.Application.Abstractions.Persistence.Identities;
using Snebur.Application.Extensions;
using Snebur.Core.Infos;
using Snebur.Domain.Entities.Identities.Factories;
using Snebur.SharedKernel.Configuration;

namespace Snebur.RuntimeServices.Services;

public class UserSessionVerificationService : IUserSessionVerificationService, IUserSessionVerificationServiceTest, IAsyncDisposable
{
    private readonly IJsonStringLocalizerCache _localizerCache;
    private readonly IIdentityUnitOfWork _unitWork;
    private readonly IHttpContextSessionAccessor _httpContextSessionAccessor;
    private readonly IUserSessionManager _userSessionManger;

    public UserSessionVerificationService(
        IJsonStringLocalizerCache localizerCache,
        IUserSessionManager userSessionManger,
        IHttpContextSessionAccessor httpContextSessionAccessor,
        IIdentityUnitOfWork unitWork)
    {
        _localizerCache = localizerCache;
        _userSessionManger = userSessionManger;
        _httpContextSessionAccessor = httpContextSessionAccessor;
        _unitWork = unitWork;
    }

    public async Task<IUserSession> VerifyAsync()
    {
        await EnsureLanguageLoadedAsync();

        var userSession = await GetValidUserSessionAsync();
        _httpContextSessionAccessor.UserSession = userSession;

        await EnsureLanguageLoadedAsync();
        return userSession;
    }

    private async Task EnsureLanguageLoadedAsync()
    {
        var language = LanguageHelper.Normalize(CultureHelper.DefaultCulture, _httpContextSessionAccessor.Language);
        await _localizerCache.EnsureLanguageLoadedAsync(language);
    }

    private async Task<IUserSession> GetValidUserSessionAsync()
    {
        var userSession = await GetUserSessionAsync();
        if (userSession is not null)
        {
            await ValidateSessionAsync(userSession);
            if (userSession.IsActive)
            {
                return userSession;
            }
        }
        return await CreateAnonymousSessionAsync();
    }

    private async Task<IUserSession?> GetUserSessionAsync()
    {
        var userSession = await _userSessionManger.GetSessionAsync();
        if (userSession is not null)
        {
            return userSession;
        }

        var session_Id = _userSessionManger.UserSession_Id;
        if (session_Id is null)
            return null;

        return await _unitWork.UserSessions.GetByIdAsync(session_Id.Value);
    }

    private async Task ValidateSessionAsync(IUserSession userSession)
    {
        if (!userSession.IsActive)
        {
            return;
        }

        if (userSession.IsAnonymous())
        {
            return;
        }

        await ValidateSessionEntityAsync(userSession);
    }

    private async Task ValidateSessionEntityAsync(IUserSession userSession)
    {
        var headerInfo = _httpContextSessionAccessor.RequestHeaderInfo;
        var terminationReason = GetSessionTerminationReason(userSession, headerInfo);
        if (terminationReason.HasValue)
        {
            var userSessionEntity = await GetUserSessionEntity(userSession);

            _httpContextSessionAccessor.UserSession = userSessionEntity;
            await TerminateSessionAsync(userSessionEntity, terminationReason.Value);
            return;
        }

        if (!userSession.IsUpdatePending())
        {
            return;
        }
        await ProcessSessionUpdateAsync(userSession);
    }

    private async Task<UserSession?> GetUserSessionEntity(IUserSession userSession)
    {
        if (userSession is UserSession entity)
        {
            return entity;
        }

        return await _unitWork.UserSessions.GetByIdAsync(userSession.Id);
    }

    private SessionTerminationReason? GetSessionTerminationReason(
        IUserSession userSession,
        ClientRequestHeaderInfo headerInfo)
    {
        if (userSession.IsExpired())
        {
            return SessionTerminationReason.SessionExpired;
        }

        if (!string.Equals(userSession.IpAddress, headerInfo.IpAddress, StringComparison.OrdinalIgnoreCase))
        {
            return SessionTerminationReason.IpAddressChanged;
        }

        if (!string.Equals(userSession.UserAgent, headerInfo.UserAgent, StringComparison.OrdinalIgnoreCase))
        {
            return SessionTerminationReason.UserAgentChanged;
        }
        return null;
    }

    private async Task TerminateSessionAsync(
        UserSession? userSession,
        SessionTerminationReason reason)
    {
        if (userSession is null || !userSession.IsActive)
            return;

        userSession.TerminateSession(reason);
        await _unitWork.SaveChangesAsync();
        await _userSessionManger.RemoveSessionAsync();
    }

    private async Task ProcessSessionUpdateAsync(IUserSession userSession)
    {
        var userSessionEntity = await GetUserSessionEntity(userSession);
        if (userSessionEntity is null || !userSessionEntity.IsActive)
        {
            return;
        }

        userSessionEntity.UpdateLastActivity();

        _unitWork.Update(userSessionEntity);

        var result = await _unitWork.SaveChangesAsync(silent: true);
        if (result.IsFailure)
        {
            var terminationReason = result.Error is DomainEventError
                ? SessionTerminationReason.DomainEventError
                : throw result.Exception;

            await TerminateSessionAsync(userSessionEntity, terminationReason);
        }

        var needsRefreshToken = NeedsRefreshSession(userSessionEntity);
        if (needsRefreshToken)
        {
            var user = await _unitWork.GetUserAsync(userSessionEntity.User_Id, userSessionEntity.UserType);
            if (user is null)
            {
                throw new CriticalNotFoundException("User not found.");
            }
            await _userSessionManger.SetSessionAsync(userSessionEntity, user);
        }
    }

    private bool NeedsRefreshSession(UserSession session)
    {
        var expiration = _httpContextSessionAccessor.UserSessionClaims?.Expiration;
        if (expiration is null)
        {
            throw new InvalidOperationException("Expiration is null");
        }
        return UserSessionConfig.NeedsRefreshSession(expiration.Value, session.IsPersistent);
    }

    private async Task<UserSession> CreateAnonymousSessionAsync()
    {
        var headerInfo = _httpContextSessionAccessor.RequestHeaderInfo;
        var anonymousUser = AnonymousUserConstants.AnonymousUser;

        var userSession = UserSessionFactory.Create(
              user: anonymousUser,
              clientHeaderInfo: headerInfo,
              authenticationType: AuthenticationType.Anonymous,
              isPersistent: true,
              tenant_id: null);

        _unitWork.Add(userSession);

        var result = await _unitWork.SaveChangesAsync();
        if (result.IsFailure)
        {
            throw new InvalidOperationException("Error while creating anonymous session.", result.Exception);
        }

        await _userSessionManger.SetSessionAsync(userSession, anonymousUser);
        return userSession;
    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return _unitWork.DisposeAsync();
    }

    #region IUserSessionVerificationServiceTest

    Task<UserSession> IUserSessionVerificationServiceTest.CreateAnonymousSessionAsync()
    {
        return CreateAnonymousSessionAsync();
    }

    #endregion

}
