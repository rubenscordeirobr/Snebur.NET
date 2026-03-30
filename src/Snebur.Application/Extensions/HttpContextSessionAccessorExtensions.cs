namespace Snebur.Application.Extensions;

public static class HttpContextSessionAccessorExtensions
{
    public static IUserSession GetRequiredUserSession(
        this IHttpContextSessionAccessor httpContextSessionAccessor)
    {
        Guard.NotNull(httpContextSessionAccessor);

        var userSession = httpContextSessionAccessor.UserSession;
        if (userSession is null)
        {
            return AnonymousUserConstants.AnonymousUserSession;
        }
        return userSession;
    }
}

