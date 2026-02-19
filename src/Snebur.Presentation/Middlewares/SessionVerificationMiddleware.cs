using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Snebur.Presentation.Middlewares;

public sealed class SessionVerificationMiddleware
{
    private readonly RequestDelegate _next;

    public SessionVerificationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        Guard.NotNull(context);

        var serviceProvider = context.RequestServices;
        var logger = serviceProvider.GetRequiredService<ILogger<SessionVerificationMiddleware>>();
        try
        {
            var sessionVerification = serviceProvider.GetService<IUserSessionVerificationService>();
            if (sessionVerification == null)
            {
                logger.LogError("UserSessionVerificationService not registered.");
                return;
            }
            var userSession = await sessionVerification.VerifyAsync();
            if (userSession is null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving UserSessionVerificationService.");
        }
        await _next(context);
    }
}
