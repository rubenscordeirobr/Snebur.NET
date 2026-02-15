using System.Reflection;
using Snebur.UseCases.Common.Registrars;
using Microsoft.Extensions.DependencyInjection;

namespace Snebur.UseCases;

public static class SharedUseCasesServiceConfiguration
{
    public static IServiceCollection AddUserCasesSharedServices(
        this IServiceCollection services )
    {

        ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Stop;
        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
         
        services.AddCommandValidationServicesFromAssembly(Assembly.GetExecutingAssembly());
        return services;
    }

    public static IServiceCollection AddCommandValidationServicesFromAssembly(
       this IServiceCollection services,
       Assembly assembly)
    {
        Guard.NotNull(assembly);

        var validationRegistrar = new CommandValidationRegistrar(services);
        validationRegistrar.RegisterFromAssembly(assembly);
        return services;
    }

    internal static IServiceCollection AddCommandValidationServicesFromTypes(
        this IServiceCollection services,
        Type[] types)
    {
        var validationRegistrar = new CommandValidationRegistrar(services);
        validationRegistrar.RegisterFromTypes(types);
        return services;
    }
}
