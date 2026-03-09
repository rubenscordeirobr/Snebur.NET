using Snebur.Domain.Entities.Identities.Events;

namespace Snebur.UseCases.Identities.Authentications.Commands;

public class TenantUserLogoutCommandHandler : CommandHandler<TenantUserLogoutCommand, OperationResponse>
{
    private readonly IIdentityUnitOfWork _unitOfWork;
    private readonly IUserSessionManager _userSessionManager;
    private readonly IHttpContextSessionAccessor _httpContextSessionAccessor;
    private readonly IEventMediator _eventMediator;

    public TenantUserLogoutCommandHandler(
        IIdentityUnitOfWork unitOfWork,
        IUserSessionManager userSessionManager,
        IHttpContextSessionAccessor httpContextSessionAccessor,
        IEventMediator eventMediator)
    {
        _unitOfWork = unitOfWork;
        _userSessionManager = userSessionManager;
        _httpContextSessionAccessor = httpContextSessionAccessor;
        _eventMediator = eventMediator;
    }

    protected override async Task<Result<OperationResponse>> HandleAsync(
        TenantUserLogoutCommand command,
        CancellationToken cancellationToken)
    {
        Guard.NotNull(command);

        var session_Id = command.Session_Id;

        if (_userSessionManager.UserSession_Id != session_Id)
        {
            return Result.Failure<OperationResponse>(
                    new ForbiddenError(
                        "UserSession.InvalidSessionId",
                        "The provided session ID does not match the current active session."));

        }
 
        var userSession = await _unitOfWork.UserSessions
            .GetByIdWithUserAsync(
                command.Session_Id,
                cancellationToken);

        if (userSession is null)
        {
            return Result.NotFoundFailure<OperationResponse>(
                "UserSession.NotFound",
                $"UserSession with token {session_Id} not found.");
        }

        if (userSession.User is not TenantUser _)
        {
            return Result.Failure<OperationResponse>(
                new InvalidOperationError(
                    "UserSession.InvalidUserType",
                    "User is not a tenant user."));
        }

        if (userSession.IsActive)
        {
            userSession.TerminateSession(SessionTerminationReason.UserLogout);
            _unitOfWork.Update(userSession);

            var result = await _unitOfWork.SaveChangesAsync(silent: true, cancellationToken);
            if (result.IsFailure)
            {
                return Result.Failure<OperationResponse>(result.Error);
            }
        }

        var headerInfo = _httpContextSessionAccessor.RequestHeaderInfo;
        var logoutEvent = new UserLoggedOutEvent(userSession.User, headerInfo.IpAddress);

        await _eventMediator.DispatchAsync(userSession, logoutEvent);
        await _userSessionManager.RemoveSessionAsync();

        return Result.Success(new OperationResponse());
    }
}
