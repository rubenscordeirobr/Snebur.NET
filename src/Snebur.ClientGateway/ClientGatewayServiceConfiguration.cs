using Snebur.ClientGateway.Common;
using Snebur.ClientGateway.Identities;
using Snebur.Core.Helpers;
using Snebur.SharedKernel.Abstractions;
using Snebur.SharedKernel.Extensions;
using Snebur.SharedKernel.Localization;
using Snebur.UseCases;
using Snebur.UseCases.Identities.Authentications.Commands;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Snebur.ClientGateway;

public static class ClientGatewayServiceConfiguration
{
    public static IServiceCollection AddClientGatewayServices(
        this IServiceCollection services)
    {
        var isDevelopment = EnvironmentHelper.IsDevelopment();
        var localizerConfiguration = new JsonLocalizationCacheConfiguration
        {
            AutoSeedMissingLocalization = isDevelopment,
            AutoUpdateDefaultKeys = isDevelopment,
            AutoTranslate = isDevelopment,
            CustomTranslationModelId = null
        };

        services.AddSharedKernelServices()
            .AddUserCasesSharedServices();

        services.AddSingleton<IHttpClientResilienceOptions, HttpClientResilienceOptionsDefault>()
            .AddSingleton(localizerConfiguration);

        services.AddScoped<ITenantService, TenantService>()
            .AddScoped<ITenantValidationService, TenantValidationService>()
            .AddScoped<ITenantUserService, TenantUserService>()
            .AddScoped<ITenantUserValidationService, TenantUserValidationService>()
            .AddScoped<IJsonStringLocalizerService, JsonStringLocalizerService>()
            .AddScoped(typeof(IHttpClientMediator<>), typeof(HttpClientMediator<>))
            .AddTransient<IHttpClientExecutor, HttpClientExecutor>();

        services.Remove<IValidator<TenantUserLoginCommand>>();
        services.Remove<IValidator<TenantUserLogoutCommand>>();

        services.Remove<IValidator<AdminUserLoginCommand>>();
        services.Remove<IValidator<AdminUserLogoutCommand>>();

        return services;
    }

    public static IServiceCollection AddTenantUserAuthenticationServices(
       this IServiceCollection services)
    {
        return services.AddScoped<ITenantUserAuthenticationService, TenantUserAuthenticationService>()
            .AddScoped<ITenantUserAuthenticationValidationService, TenantUserAuthenticationValidationService>()
            .AddTransient<IValidator<TenantUserLoginCommand>, TenantUserLoginCommandValidator>()
            .AddTransient<IValidator<TenantUserLogoutCommand>, TenantUserLogoutCommandValidator>();
    }

    public static IServiceCollection AddAdminUserAuthenticationServices(
       this IServiceCollection services)
    {
        return services.AddScoped<IAdminUserAuthenticationService, AdminUserAuthenticationService>()
            .AddScoped<IAdminUserAuthenticationValidationService, AdminUserAuthenticationValidationService>()
            .AddScoped<IAdminUserService, AdminUserService>()
            .AddTransient<IValidator<AdminUserLoginCommand>, AdminUserLoginCommandValidator>()
            .AddTransient<IValidator<AdminUserLogoutCommand>, AdminUserLogoutCommandValidator>();
    }

    public static async Task InitializeClientServiceAsync(
      this IServiceProvider serviceProvider)
    {
        var cache = serviceProvider.GetRequiredService<IJsonStringLocalizerCache>();
        await cache.EnsureLanguageLoadedAsync(LanguageHelper.SystemLanguage);
    }
}
