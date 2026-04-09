using Snebur.SharedKernel.Abstractions;
using Snebur.SharedKernel.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace Snebur.SharedKernel;

public static class SharedKernelServiceConfiguration
{
    public static IServiceCollection AddSharedKernelServices(
        this IServiceCollection services )
    {
        services
            .AddSingleton<IJsonStringLocalizerCache, JsonStringLocalizerCache>()
            .AddScoped(typeof(IJsonStringLocalizer<>), typeof(JsonStringLocalizer<>));

        return services;
    }
}
