using Snebur.Domain.Entities.Identities.Events;
using Snebur.Domain.Entities.Identities.Factories;
using Snebur.UseCases.Mappers.Identities;

namespace Snebur.UseCases.Identities.Authentications.Commands;

public class AdminUserLoginCommandHandler : CommandHandler<AdminUserLoginCommand, AdminUserLoginResponse>
{
    private readonly IIdentityUnitOfWork _unitOfWork;
    private readonly IHttpContextSessionAccessor _httpContextSessionAccessor;
    private readonly IUserSessionManager _userSessionManager;
    private readonly ISecureConfiguration _secureConfiguration;
    private readonly IAuthenticationAttemptLimiterService _authenticationLimiter;
    private readonly IEventMediator _eventMediator;
    private readonly ILogger<AdminUserLoginCommandHandler> _logger;

    public AdminUserLoginCommandHandler(
        IIdentityUnitOfWork unitOfWork,
        ISecureConfiguration secureConfiguration,
        IUserSessionManager userSessionManager,
        IHttpContextSessionAccessor httpContextSessionAccessor,
        IAuthenticationAttemptLimiterService authenticationValidator,
        IEventMediator eventMediator,
        ILogger<AdminUserLoginCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _secureConfiguration = secureConfiguration;
        _userSessionManager = userSessionManager;
        _httpContextSessionAccessor = httpContextSessionAccessor;
        _authenticationLimiter = authenticationValidator;
        _eventMediator = eventMediator;
        _logger = logger;
    }

    protected override async Task<Result<AdminUserLoginResponse>> HandleAsync(
        AdminUserLoginCommand command,
        CancellationToken cancellationToken)
    {
        Guard.NotNull(command);

        var headerInfo = _httpContextSessionAccessor.RequestHeaderInfo;

        var maxAuthenticationResult = await _authenticationLimiter.MaxAuthenticationReachedAsync(
            headerInfo.IpAddress, cancellationToken);

        if (maxAuthenticationResult.IsMaxReached)
        {
            var totalMinutes = maxAuthenticationResult.ExpirationTime?.TotalMinutes ?? 0;
            var message = $"Too many failed login attempts. Please try again in {totalMinutes:0.0} minute(s).";

            _logger.LogWarning("The user with IP address {IpAddress} has reached the maximum number of authentication attempts.", headerInfo.IpAddress);

            return Result.Failure<AdminUserLoginResponse>(
                new TooManyRequestsError(
                    "AdminUser.MaxAuthenticationReached",
                    message));
        }

        var user = await _unitOfWork.AdminUsers
            .GetByEmailOrPhoneNumberAsync(command.EmailOrPhoneNumber, cancellationToken);

        if (user is null)
        {
            await _authenticationLimiter.IncrementFailedAttemptsAsync(headerInfo.IpAddress, cancellationToken);
            return Result.Failure<AdminUserLoginResponse>(new NotFoundError("AdminUser.NotFound", "AdminUser not found"));
        }

        var salt = _secureConfiguration.GetPasswordSalt();
        if (!PasswordHelper.VerifyPassword(command.Password, user.Password.HashValue, salt))
        {
            var currentUserSession = _httpContextSessionAccessor.GetRequiredUserSession();
            var loginFailedEvent = new UserLoginFailedEvent(
                user, 
                command.EmailOrPhoneNumber,
                command.Password,
                headerInfo.IpAddress);

            await _eventMediator.DispatchAsync(currentUserSession, loginFailedEvent);

            await _authenticationLimiter.IncrementFailedAttemptsAsync(headerInfo.IpAddress, cancellationToken);
            return Result.Failure<AdminUserLoginResponse>(
                new AuthenticationError("AdminUser.InvalidPassword", "Invalid password"));
        }

        if (cancellationToken.IsCancellationRequested)
        {

            return Result.Failure<AdminUserLoginResponse>(
                new OperationCanceledError(null,
                    "AdminUserLoginCommandHandler.HandleAsync",
                    "Operation was canceled."));
        }

        var newUserSession = UserSessionFactory.Create(
            user: user,
            clientHeaderInfo: headerInfo,
            authenticationType: AuthenticationType.Credentials,
            isPersistent: command.IsPersistent,
            tenant_id: null);

        _unitOfWork.Add(newUserSession);

        var result = await _unitOfWork.SaveChangesAsync(silent: true, cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<AdminUserLoginResponse>(
                new InternalServerError(result.Exception,
                    "AdminUserLoginCommandHandler.SaveChangesAsync",
                     "Failed to save user session."));
        }

        await _userSessionManager.SetSessionAsync(newUserSession, user);

        var authorizationToken = _httpContextSessionAccessor.AuthorizationToken;

        Guard.NotNullOrWhiteSpace(authorizationToken);

        var sessionResponse = UserSessionMapper.ToResponse(newUserSession);
        var userResponse = UserMapper.ToResponse(user);

        var loginSuccessEvent = new UserLoggedInEvent(
            user,
            AuthenticationType.Credentials,
            command.EmailOrPhoneNumber,
            headerInfo.IpAddress);

        await _eventMediator.DispatchAsync(newUserSession, loginSuccessEvent);

        var response = new AdminUserLoginResponse
        {
            AuthorizationToken = authorizationToken,
            User = userResponse,
            UserSession = sessionResponse
        };

        return Result.Success(response);
    }
}

