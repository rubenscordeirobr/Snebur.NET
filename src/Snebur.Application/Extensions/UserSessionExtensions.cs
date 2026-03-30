using Snebur.Application.Extensions;
using Snebur.SharedKernel.Configuration;
namespace Snebur.Application.Extensions;

public static class UserSessionExtensions
{
    public static bool IsUpdatePending(this IUserSession userSession)
    {
        Guard.NotNull(userSession);

        return userSession.IsActive
            && userSession.LastActivity.IsExpired(TimeSpan.FromMinutes(UserSessionConfig.UpdateCheckIntervalMinutes))
            && userSession.Id != AnonymousUserConstants.Session_Id;
    }

    public static bool IsExpired(this IUserSession userSession)
    {
        Guard.NotNull(userSession);

        if (userSession.IsAnonymous())
        {
            return false;
        }

        var expirationTime = UserSessionConfig.GetSessionExpiration(userSession.IsPersistent);
        return DateTime.Now > userSession.LastActivity.Add(expirationTime);
    }

    public static bool IsAnonymous(this IUserSession userSession)
    {
        Guard.NotNull(userSession);

        return userSession.User_Id == AnonymousUserConstants.User_Id ||
            userSession.AuthenticationType == AuthenticationType.Anonymous ||
            userSession.UserType == UserType.Anonymous;
    }

    public static bool IsTenantUser(this IUserSession userSession)
    {
        Guard.NotNull(userSession);

        return userSession.Tenant_Id.HasValue
            && userSession.Tenant_Id != Guid.Empty;
    }

    public static bool IsSystemAdminUser(this IUserSession userSession)
    {
        Guard.NotNull(userSession);

        if (userSession.IsAnonymous())
        {
            return false;
        }

        if (userSession.UserType is UserType.AdminUser or UserType.SystemUser)
        {
            return userSession.UserRole is UserRole.Admin or UserRole.SystemAdmin;
        }
        return false;

    }
}
