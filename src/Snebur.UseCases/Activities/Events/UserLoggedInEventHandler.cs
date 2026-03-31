using Snebur.Application.Abstractions.Persistence.Activities;
using Snebur.Domain.Entities.Activities;
using Snebur.Domain.Entities.Identities.Events;

namespace Snebur.UseCases.Activities.Events;

public sealed class UserLoggedInEventHandler : IDomainEventHandler<UserLoggedInEvent >
{
    private readonly IActivityRepository _activityRepository;
    private readonly IHttpContextSessionAccessor _userSessionAccessor;
    private readonly ILogger<UserLoggedInEventHandler> _logger;

    public UserLoggedInEventHandler(
        IActivityRepository activityRepository,
        IHttpContextSessionAccessor userSessionAccessor,
        ILogger<UserLoggedInEventHandler> logger)
    {
        _activityRepository = activityRepository;
        _userSessionAccessor = userSessionAccessor;
        _logger = logger;
    }

    public async Task HandleAsync(UserLoggedInEvent  domainEvent, CancellationToken cancellationToken = default)
    {
        Guard.NotNull(domainEvent);

        var userSession = _userSessionAccessor.GetRequiredUserSession();
        var user = domainEvent.User;

        var description = $"User {user.Name} logged in by credentials. UserIdentifier {domainEvent.UserIdentifier }";

        var activity = new UserLoginSuccessActivity
        {
            AuthenticationType = domainEvent.AuthenticationType,
            Tenant_Id = userSession.Tenant_Id,
            UserSession_Id = userSession.Id,
            ActivityDate = DateTime.UtcNow,
            Description = description,
            IpAddress = domainEvent.IpAddress,
            User_Id = user.Id,
            UserIdentifier  = domainEvent.UserIdentifier 
        };

        try
        {
            await _activityRepository.AddAsync(activity, cancellationToken  );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                  "Failed to add activity [{ActivityType}] for user type [{UserType}] with ID [{UserId}]. " +
                  "Tenant ID: {TenantId}, Session ID: {SessionId}, IP: {IpAddress}",
                  nameof(UserLogoutActivity),
                  user.GetType().Name,
                  user.Id,
                  userSession.Tenant_Id,
                  userSession.Id,
                  domainEvent.IpAddress);
            throw;
        }
    }
}
