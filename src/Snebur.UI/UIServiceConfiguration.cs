using System.Reflection;
using Snebur.ClientGateway;
using Snebur.ClientGateway.Abstractions;
using Snebur.ClientGateway.Common.Abstractions;
using Snebur.UI.Core;
using Snebur.UI.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Snebur.UI;

public static class UIServiceConfiguration
{
    public static IServiceCollection AddSneburUIServices(
       this IServiceCollection services)
    {
        return services.AddCascadingAuthenticationState()
            .AddFluentUIComponents()
            .AddClientGatewayServices()
            .AddScoped<IInternetStatusService, InternetStatusService>()
            .AddScoped<IConnectionStatusNotifier, ConnectionStatusNotifier>()
            .AddScoped<IRequestErrorNotifier, RequestErrorNotifier>()
            .AddScoped<ICultureProvider, CultureProvider>()
            .AddScoped<LocalizedNavigationManager>()
            .AddScoped<IRouteService, RouteService>()
            .AddScoped<IClientAuthorizationTokenManager, ClientAuthorizationTokenManager>()
            .AddScoped<IBusyIndicatorService, BusyIndicatorService>()
            .AddScoped<IThemeService, ThemeService>();
    }

    public static async Task InitializeUIServiceAsync(
       this IServiceProvider serviceProvider)
    {
        await serviceProvider.InitializeClientServiceAsync();
    }

    public static void AddViewModelsAndValidators(
        this IServiceCollection services,
        Assembly assembly)
    {
        Guard.NotNull(assembly);

        var viewModelTypes = assembly.GetTypes()
            .Where(type => type.IsSubclassOf(typeof(ViewModelBase)))
            .ToArray();

        foreach (var viewModelType in viewModelTypes)
        {
            services.AddTransient(viewModelType);

            var validatorType = typeof(IValidator<>).MakeGenericType(viewModelType);
            var validator = assembly.GetTypes()
                .FirstOrDefault(type => validatorType.IsAssignableFrom(type));
           
            if (validator != null)
            {
                services.AddTransient(validatorType, validator);
            }
        }
    }
}
