using System.Reflection;
using Snebur.Application;
using Snebur.UseCases.Identities.Authentications.Services;
using Snebur.UseCases.Identities.Tenants.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Snebur.UseCases;

public static class UseCasesServiceConfiguration
{
    public static IServiceCollection AddUserCasesServices(
        this IServiceCollection services)
    {
        services.AddApplicationHandlersFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<ITenantValidationService, TenantValidationService>();
        services.AddScoped<ITenantUserValidationService, TenantUserValidationService>();
        services.AddScoped<ITenantUserAuthenticationValidationService, TenantUserAuthenticationValidationService>();
        services.AddScoped<IAdminUserAuthenticationValidationService, AdminUserAuthenticationValidationService>();

        return services;
    }

}
