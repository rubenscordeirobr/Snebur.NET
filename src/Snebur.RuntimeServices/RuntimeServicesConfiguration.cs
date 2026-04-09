using Snebur.Application.Abstractions.Security;
using Snebur.RuntimeServices.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace Snebur.RuntimeServices;

public static class RuntimeServicesConfiguration
{
    public static IServiceCollection AddRuntimeServices(
        this IServiceCollection services)
    {
        services.AddSingleton<ICommandTrackingService, CommandTrackingService>()
            .AddSingleton<IUserSessionCacheService, UserSessionCacheService>()
            .AddSingleton<IUserSessionTokenHandler, UserSessionTokenHandler>()
            .AddSingleton<IEntityAuthorizationService, EntityAuthorizationService>()
            .AddSingleton<IAuthenticationAttemptLimiterService, AuthenticationAttemptLimiterService>()
            .AddSingleton<IJsonStringLocalizerService, JsonStringLocalizerService>()
            .AddSingleton<ITranslationService, LibreTranslationService>()
            .AddScoped<IRequestMediator, RequestMediator>()
            .AddScoped<IEventMediator, EventMediator>()
            .AddScoped<IUserSessionManager, UserSessionManager>()
            .AddScoped<ICultureProvider, CultureProvider>()
            .AddTransient<IUserSessionVerificationService, UserSessionVerificationService>();

        return services;
    }     
}
