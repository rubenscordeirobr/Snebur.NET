using Snebur.Application.Extensions;
using Snebur.Presentation.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Snebur.Presentation;

public static class PresentationServiceConfiguration
{
    public static IServiceCollection AddPresentationServices(
        this IServiceCollection services)
    {
        services.AddScoped<IHttpContextSessionAccessor, HttpContextSessionAccessor>()
            .AddTransient(provider =>
            {
                var accessor = provider.GetRequiredService<IHttpContextSessionAccessor>();
                return accessor.GetRequiredUserSession();
            });
        return services;
    }
}
