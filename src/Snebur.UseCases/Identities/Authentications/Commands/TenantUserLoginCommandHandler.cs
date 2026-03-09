using Snebur.Domain.Entities.Identities.Events;
using Snebur.Domain.Entities.Identities.Factories;
using Snebur.UseCases.Mappers.Identities;

namespace Snebur.UseCases.Identities.Authentications.Commands;

public class TenantUserLoginCommandHandler : CommandHandler<TenantUserLoginCommand, TenantUserLoginResponse>
{
    private readonly IIdentityUnitOfWork _unitOfWork;
    private readonly IHttpContextSessionAccessor _httpContextSessionAccessor;
    private readonly IUserSessionManager _userSessionManager;
    private readonly ISecureConfiguration _secureConfiguration;
    private readonly IAuthenticationAttemptLimiterService _authenticationLimiter;
    private readonly IEventMediator _eventMediator;
    private readonly ILogger<TenantUserLoginCommandHandler> _logger;

    public TenantUserLoginCommandHandler(
        IIdentityUnitOfWork unitOfWork,
        ISecureConfiguration secureConfiguration,
        IUserSessionManager userSessionManager,
        IHttpContextSessionAccessor httpContextSessionAccessor,
        IAuthenticationAttemptLimiterService authenticationValidator,
        IEventMediator eventMediator,
        ILogger<TenantUserLoginCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _secureConfiguration = secureConfiguration;
        _userSessionManager = userSessionManager;
        _httpContextSessionAccessor = httpContextSessionAccessor;
        _authenticationLimiter = authenticationValidator;
        _eventMediator = eventMediator;
        _logger = logger;
    }

    protected override async Task<Result<TenantUserLoginResponse>> HandleAsync(
        TenantUserLoginCommand command,
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

            return Result.Failure<TenantUserLoginResponse>(
                new TooManyRequestsError(
                    "TenantUser.MaxAuthenticationReached",
                    message));
        }

        var user = await _unitOfWork.TenantUsers
            .GetByEmailOrPhoneNumberAsync(command.EmailOrPhoneNumber, cancellationToken);

        if (user is null)
        {
            await _authenticationLimiter.IncrementFailedAttemptsAsync(headerInfo.IpAddress, cancellationToken);
            return Result.Failure<TenantUserLoginResponse>(new NotFoundError("TenantUser.NotFound", "Tenant user not found"));
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
            return Result.Failure<TenantUserLoginResponse>(
                new AuthenticationError("TenantUser.InvalidPassword", "Invalid password"));
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return Result.Failure<TenantUserLoginResponse>(
                new OperationCanceledError(null,
                    "TenantUserLoginCommandHandler.HandleAsync",
                    "Operation was canceled."));
        }

        var tenant = await _unitOfWork.Tenants.NoTracking().GetByIdAsync(user.Tenant_Id, cancellationToken);
        if (tenant is null)
        {
            return Result.Failure<TenantUserLoginResponse>(
                new CriticalNotFoundError("TenantUser.TenantNotFound", $"Tenant Id {user.Tenant_Id} from TenantUser Id {user.Id} not found."));
        }

        if (!tenant.IsActive())
        {
            return Result.Failure<TenantUserLoginResponse>(
                new AuthenticationError("TenantUser.TenantInactive", "Tenant is inactive."));
        }

        var newUserSession = UserSessionFactory.Create(
            user: user,
            clientHeaderInfo: headerInfo,
            authenticationType: AuthenticationType.Credentials,
            isPersistent: command.IsPersistent,
            tenant_id: user.Tenant_Id);

        _unitOfWork.Add(newUserSession);

        var result = await _unitOfWork.SaveChangesAsync(silent: true, cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<TenantUserLoginResponse>(
                new InternalServerError(result.Exception,
                "TenantUserLoginCommandHandler.SaveChangesAsync",
                "Failed to save user session."));
        }

        await _userSessionManager.SetSessionAsync(newUserSession, user);

        var authorizationToken = _httpContextSessionAccessor.AuthorizationToken;

        Guard.NotNullOrWhiteSpace(authorizationToken);

        var sessionResponse = UserSessionMapper.ToResponse(newUserSession);
        var userResponse = UserMapper.ToResponse(user);
        var tenantResponse = TenantMapper.ToResponse(tenant);

        var loginSuccessEvent = new UserLoggedInEvent(
           user,
           AuthenticationType.Credentials,
           command.EmailOrPhoneNumber,
           headerInfo.IpAddress);

        await _eventMediator.DispatchAsync(newUserSession, loginSuccessEvent);

        var response = new TenantUserLoginResponse
        {
            AuthorizationToken = authorizationToken,
            User = userResponse,
            Tenant = tenantResponse,
            UserSession = sessionResponse
        };

        return Result.Success(response);
    }
}

