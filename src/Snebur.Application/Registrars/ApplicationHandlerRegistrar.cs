using System.Reflection;
using Snebur.Application.Abstractions.Handlers;
using Snebur.Application.Abstractions.Registrars;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Snebur.Application.Registrars;

internal partial class ApplicationHandlerRegistrar
{
    private static readonly object _lock = new();
    private readonly IServiceCollection _services;
    private readonly IEventHandlerRegistryService _eventRegistryService;

    public ApplicationHandlerRegistrar(
        IServiceCollection services)
    {
        _services = services;
        _eventRegistryService = InitializeEventHandlerRegistryService();
    }

    private IEventHandlerRegistryService InitializeEventHandlerRegistryService()
    {
        lock (_lock)
        {
            using var temp = _services.BuildServiceProvider();
            var mappingService = temp.GetService<IEventHandlerRegistryService>();
            if (mappingService is not null)
            {
                return mappingService;
            }

            // Check if it's already registered but not yet resolved
            var serviceDescriptor = _services.FirstOrDefault(s => s.ServiceType == typeof(IEventHandlerRegistryService));
            if (serviceDescriptor is not null)
            {
                throw new InvalidOperationException("The EventHandlerMappingManager is already registered but not yet resolved.");
            }

            _services.AddSingleton<IEventHandlerRegistryService>(new EventHandlerRegistryService());

            return InitializeEventHandlerRegistryService();
        }
    }

    internal void RegisterFromAssembly(Assembly assembly)
    {
        var assemblyTypes = assembly.GetTypes();
        RegisterHandlerFromTypes(assemblyTypes);
    }

    internal void RegisterHandlerFromTypes(Type[] types)
    {
        RegisterRequestHandlers(types);
        RegisterEventHandlers(types);
    }

    private void RegisterRequestHandlers(Type[] assemblyTypes)
    {
        var handlerDefinitionType = typeof(IRequestHandler<,>);
        var handlersTypesMapped = TypeHelper.FindTypesImplementingInterface(assemblyTypes, handlerDefinitionType);

        foreach (var (handlerType, handlerInterfaceType) in handlersTypesMapped)
        {
            ValidatorHelper.CheckIfHasSubclassIn(handlerType, assemblyTypes);
            ValidatorHelper.CheckIfClass(handlerType);

            var requestType = handlerInterfaceType.GetGenericArguments()[0];
            var responseType = handlerInterfaceType.GetGenericArguments()[^1];

            ValidatorHelper.CheckIfNotGenericType(requestType);
            ValidatorHelper.CheckIfNotGenericType(responseType);

            TypeGuard.MustBeNotGeneric(requestType);
            TypeGuard.MustBeNotGeneric(responseType);

            TypeGuard.MustBeConcrete(responseType);
            TypeGuard.MustBeConcrete(requestType);

            var serviceType = typeof(IRequestHandler<,>)
                .MakeGenericType(requestType, responseType);

            ValidatorHelper.EnsureNoOtherHandlerRegistered(
                _services,
                serviceType,
                handlerType, 
                requestType, 
                responseType);

            _services.TryAddTransient(serviceType, handlerType);
        }
    }
     
    private void RegisterEventHandlers(Type[] assemblyTypes)
    {
        RegisterEventHandlers(
            assemblyTypes,
            typeof(IDomainEventHandler<>),
            HandlerKind.DomainEventHandler,
            _eventRegistryService.MapperDomainEventHandler);

        RegisterEventHandlers(
            assemblyTypes,
            typeof(IPreProcessorHandler<>),
            HandlerKind.PreProcessorHandler,
            _eventRegistryService.MapperDomainEventPreProcessorHandler);
    }

    private void RegisterEventHandlers(Type[] assemblyTypes,
                                       Type handlerDefinition,
                                       HandlerKind eventHandlerType,
                                       Action<Type, Type> mapperEventHandler)
    {
        var handlersTypesMapped = TypeHelper.FindTypesImplementingInterface(assemblyTypes, handlerDefinition);

        foreach (var (handlerType, handlerInterfaceType) in handlersTypesMapped)
        {
            ValidatorHelper.CheckIfHasSubclassIn(handlerType, assemblyTypes);
            ValidatorHelper.CheckIfClass(handlerType);

            var domainEventType = handlerInterfaceType.GetGenericArguments()[0];

            ValidatorHelper.ValidateDomainEventType(domainEventType);
            ValidatorHelper.ValidateHandlerType(handlerType, eventHandlerType);

            _services.TryAddTransient(handlerType, handlerType);

            mapperEventHandler.Invoke(domainEventType, handlerType);
        }
    }
}

internal enum HandlerKind
{
    RequestHandler,
    DomainEventHandler,
    PreProcessorHandler
}
