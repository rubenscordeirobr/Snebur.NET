using System.Reflection;
using Snebur.Core.Extensions;
using Snebur.Core.Helpers;
using Snebur.UseCases.Common.Excpetions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Snebur.UseCases.Common.Registrars;

internal class CommandValidationRegistrar
{
    private readonly IServiceCollection _services;

    internal CommandValidationRegistrar(
        IServiceCollection services )
    {
        _services = services;
    }

    internal void RegisterFromAssembly(Assembly assembly)
    {
        var assemblyTypes = assembly.GetTypes();
        RegisterFromTypes(assemblyTypes);
    }

    internal void RegisterFromTypes(Type[] types)
    {
        var mappedTypes = TypeHelper.FindTypesImplementingInterface(
            types,
            typeof(IValidator<>));

        foreach (var (validationType, interfaceType) in mappedTypes)
        {
            var commandType = interfaceType.GetGenericArguments()[0];
            if (validationType is null)
            {
                throw new InvalidOperationException($"Validation not found for command: {commandType.Name}");
            }

            var serviceType = typeof(IValidator<>).MakeGenericType(commandType);
            if (commandType.IsSubclassOf<ValueObjectBase>())
            {
                continue;
            }

            EnsureNoOtherCommandValidatorRegistered(
                commandType,
                serviceType,
                validationType);

            _services.TryAddTransient(serviceType, validationType);
        }
    }

    private void EnsureNoOtherCommandValidatorRegistered(
        Type commandType,
        Type serviceType,
        Type validationType)
    {
        var registeredValidators = _services.Where(
            service => service.ServiceType == serviceType &&
                        service.ImplementationType != validationType);

        if (registeredValidators.Any())
        {
            var errorMessage = GetErrorMessage(registeredValidators, commandType, serviceType, validationType);
            throw new CommandValidatorAlreadyExistsException(errorMessage);
        }
    }

    private string GetErrorMessage(IEnumerable<ServiceDescriptor> registeredValidators,
        Type commandType,
        Type serviceType,
        Type validationType)
    {
        var implementationType = registeredValidators.First().ImplementationType!;
        return $"The service  {serviceType.GetQualifiedName()} for the command {commandType.GetQualifiedName()} can't be registered for " +
               $" the implementation {validationType.GetQualifiedName()} because it is already registered for " +
               $"{implementationType.GetQualifiedName()}";
    }
}
