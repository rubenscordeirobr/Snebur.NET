using Microsoft.Extensions.DependencyInjection;

namespace Snebur.SharedKernel.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection Remove<T>(
        this IServiceCollection services)
    {
        Guard.NotNull(services);

        var typeRemove = typeof(T);
        var descriptorsToRemove = services
            .Where(descriptor => descriptor.ServiceType == typeRemove)
            .ToList();

        foreach (var descriptor in descriptorsToRemove)
        {
            services.Remove(descriptor);
        }
        return services;

    }
}
