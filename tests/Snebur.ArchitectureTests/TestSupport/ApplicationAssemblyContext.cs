using Snebur.Application.Abstractions.Handlers;
using Snebur.Domain.Primitives;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Snebur.ArchitectureTests.TestSupport;

public class ApplicationAssemblyContext
{
    private readonly Lock _lock = new();
    public Assembly CommonAssembly { get; }
    public Assembly DomainAssembly { get; }
    public Assembly SharedKernelAssembly { get; }
    public Assembly ApplicationAssembly { get; }
    public Assembly UseCasesAssembly { get; }
    public Assembly UseCasesSharedAssembly { get; }
    public Assembly RuntimeServicesAssembly { get; }
    public Assembly PresentationAssembly { get; }
    public Assembly ClientGatewayAssembly { get; }

    public Assembly InfrastructureAssembly { get; }
    public Assembly IdentityPersistenceAssembly { get; }
    public Assembly ActivityPersistenceAssembly { get; }
     
    public Assembly[] InfrastructuresAssemblies { get; }
    public Assembly[] AllAssemblies { get; }
    public string[] InfrastructuresAssemblyNames { get; }

    public string CommonAssemblyName
        => CommonAssembly.GetName().Name!;

    public string DomainAssemblyName
        => DomainAssembly.GetName().Name!;

    public string ApplicationAssemblyName
        => ApplicationAssembly.GetName().Name!;

    public string UseCasesAssemblyName
        => UseCasesAssembly.GetName().Name!;

    public string PresentationAssemblyName
        => PresentationAssembly.GetName().Name!;

    public string InfrastructureAssemblyName
        => InfrastructureAssembly.GetName().Name!;

    public string IdentityPersistenceAssemblyName
        => IdentityPersistenceAssembly.GetName().Name!;

    public string ActivityPersistenceAssemblyName
        => ActivityPersistenceAssembly.GetName().Name!;

    public string SharedKernelAssemblyName
        => SharedKernelAssembly.GetName().Name!;

    public string UseCasesSharedAssemblyName
        => UseCasesSharedAssembly.GetName().Name!;

    public string ClientGatewayAssemblyName
        => ClientGatewayAssembly.GetName().Name!;

    public IReadOnlyList<Type> Types
    {
        get { lock (_lock) { return field ??= GetAllTypes(); } }
    }

    public IReadOnlyList<Type> ServiceTypes
    {
        get { lock (_lock) { return field ??= GetServiceTypes(); } }
    }

    public IReadOnlyList<Type> ImplementServiceTypes
    {
        get { lock (_lock) { return field ??= GetImplementServiceTypes(); } }
    }

    public IReadOnlyList<Type> CommandTypes
    {
        get { lock (_lock) { return field ??= GetCommandTypes(); } }
    }

    public IReadOnlyDictionary<Type, IReadOnlyList<Type>> CommandTypeToValidatorTypeMappings
    {
        get { lock (_lock) { return field ??= GetCommandValidatorsMappings(); } }
    }

    public IReadOnlyDictionary<Type, Type> CommandTypeToValidatorMap
    {
        get { lock (_lock) { return field ??= GetCommandValidatorMap(); } }
    }

    public IReadOnlyList<Type> EntityTypes
    {
        get { lock (_lock) { return field ??= GetEntityTypes(); } }
    }

    public IReadOnlyList<Type> EntityConfigurationTypes
    {
        get { lock (_lock) { return field ??= GetEntityConfigurationTypes(); } }
    }

    public IReadOnlyDictionary<Type, Type> EntityTypeToConfigurationTypeMap
    {
        get { lock (_lock) { return field ??= GetEntityTypeToConfigurationTypeMap(); } }
    }

    public IReadOnlyList<Type> RequestTypes
    {
        get { lock (_lock) { return field ??= GetRequestTypes(); } }
    }

    public IReadOnlyDictionary<Type, Type> RequestTypeToHandlerTypeMap
    {
        get { lock (_lock) { return field ??= GetRequestTypeToHandlerTypeMap(); } }
    }

#pragma warning disable CS9264
    public ApplicationAssemblyContext()
#pragma warning restore CS9264
    {
        CommonAssembly = typeof(Guard).Assembly;
        DomainAssembly = typeof(EntityBase).Assembly;

        ApplicationAssembly = typeof(Application.ApplicationServiceConfiguration).Assembly;

        UseCasesAssembly = typeof(UseCases.UseCasesServiceConfiguration).Assembly;
        SharedKernelAssembly = typeof(SharedKernel.SharedKernelServiceConfiguration).Assembly;
        UseCasesSharedAssembly = typeof(UseCases.SharedUseCasesServiceConfiguration).Assembly;

        RuntimeServicesAssembly = typeof(RuntimeServices.RuntimeServicesConfiguration).Assembly;

        PresentationAssembly = typeof(Presentation.PresentationServiceConfiguration).Assembly;
        ClientGatewayAssembly = typeof(ClientGateway.ClientGatewayServiceConfiguration).Assembly;

        InfrastructureAssembly = typeof(Infrastructure.InfrastructureServiceConfiguration).Assembly;
        IdentityPersistenceAssembly = typeof(Persistence.Identity.IdentityPersistenceServiceConfiguration).Assembly;
        ActivityPersistenceAssembly = typeof(Persistence.Activity.ActivityPersistenceServiceConfiguration).Assembly;

        InfrastructuresAssemblies = [
            InfrastructureAssembly,
            IdentityPersistenceAssembly,
            ActivityPersistenceAssembly
        ];

        InfrastructuresAssemblyNames = [
            InfrastructureAssemblyName,
            IdentityPersistenceAssemblyName,
            ActivityPersistenceAssemblyName!
        ];

        AllAssemblies = [
            CommonAssembly,
            DomainAssembly,
            ApplicationAssembly,
            UseCasesAssembly,
            RuntimeServicesAssembly,
            PresentationAssembly,
            InfrastructureAssembly,
            IdentityPersistenceAssembly,
            ActivityPersistenceAssembly,
            SharedKernelAssembly,
            UseCasesSharedAssembly,
            ClientGatewayAssembly,
        ];

        if (AllAssemblies.Any(a => a == null || a.GetName().Name.IsNullOrWhiteSpace()))
        {
            throw new InvalidOperationException("One or more assemblies are null.");
        }
    }

    private IReadOnlyList<Type> GetAllTypes()
    {
        return [.. AllAssemblies.SelectMany(a => a.GetTypes())];
    }

    private IReadOnlyList<Type> GetServiceTypes()
    {
        return [.. Types.Where(ServiceReflectionHelper.IsImplementService)];
    }
     
    private IReadOnlyList<Type> GetImplementServiceTypes()
    {
        return [.. ServiceTypes.Where(t => t.IsConcrete())];
    }

    private IReadOnlyList<Type> GetCommandTypes()
    {
        return [.. Types
            .Where(x => x.IsConcrete())
            .Where(t => t.ImplementsGenericInterfaceDefinition(typeof(ICommandRequest<>)))];
    }

    private IReadOnlyDictionary<Type, IReadOnlyList<Type>> GetCommandValidatorsMappings()
    {
        var validatorTypes = Types
            .Where(x => x.IsConcrete())
            .Where(t => t.ImplementsGenericInterfaceDefinition(typeof(IValidator<>)));

        var dictionary = new Dictionary<Type, List<Type>>();
        foreach (var validatorType in validatorTypes)
        {
            var commandType = validatorType.GetInterfaces()
                .Where(x => x.IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(IValidator<>))
                .Select(x => x.GetGenericArguments()[0])
                .First();

            if (!dictionary.ContainsKey(commandType))
            {
                dictionary[commandType] = [];
            }
            dictionary[commandType].Add(validatorType);
        }

        var readOnlyListDictionary = dictionary.ToDictionary(
              kvp => kvp.Key,
              kvp => (IReadOnlyList<Type>)kvp.Value);

        return readOnlyListDictionary.AsReadOnly();
    }

    private IReadOnlyDictionary<Type, Type> GetCommandValidatorMap()
    {
        return CommandTypeToValidatorTypeMappings.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value[0])
            .AsReadOnly();
    }

    private IReadOnlyList<Type> GetEntityTypes()
    {
        return [.. Types
            .Where(x => x.IsConcrete())
            .Where(t => t.IsSubclassOf(typeof(EntityBase)))];
    }

    private IReadOnlyList<Type> GetEntityConfigurationTypes()
    {
        return [.. Types
            .Where(x => x.IsConcrete())
            .Where(t => t.ImplementsGenericInterfaceDefinition(typeof(IEntityTypeConfiguration<>)))];
    }

    private IReadOnlyDictionary<Type, Type> GetEntityTypeToConfigurationTypeMap()
    {
        var dictionary = new Dictionary<Type, Type>();
        var entityConfigurationTypes = EntityConfigurationTypes;

        foreach (var configurationEntityType in entityConfigurationTypes)
        {
            var entityType = configurationEntityType.GetInterfaces()
                .Where(x => x.IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>))
                .Select(x => x.GetGenericArguments()[0])
                .First();
            dictionary[entityType] = configurationEntityType;
        }

        return dictionary.AsReadOnly();
    }

    private IReadOnlyList<Type> GetRequestTypes()
    {
        return [.. Types
            .Where(x => x.IsConcrete())
            .Where(t => t.ImplementsGenericInterfaceDefinition(typeof(IRequest<>)))];
    }

    private IReadOnlyDictionary<Type, Type> GetRequestTypeToHandlerTypeMap()
    {
        var dictionary = new Dictionary<Type, Type>();
        var handlerTypes = Types
            .Where(x => x.IsConcrete())
            .Where(t => t.ImplementsGenericInterfaceDefinition(typeof(IRequestHandler<,>)));

        foreach (var handlerType in handlerTypes)
        {
            var requestType = handlerType.GetInterfaces()
                .Where(x => x.IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))
                .Select(x => x.GetGenericArguments()[0])
                .First();

            dictionary[requestType] = handlerType;
        }
        return dictionary.AsReadOnly();
    }
}
