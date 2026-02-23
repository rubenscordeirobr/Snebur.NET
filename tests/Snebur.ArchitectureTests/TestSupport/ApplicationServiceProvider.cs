using Snebur.Application.Abstractions.Events;
using Snebur.Domain.Primitives;
using Microsoft.Extensions.DependencyInjection;

namespace Snebur.ArchitectureTests.TestSupport;

public class ApplicationServiceProvider : AbstractTestOutputServiceProvider
{
    private readonly IServiceProvider _serviceProvider;
    public IServiceCollection Services { get; }
    protected override IServiceProvider ServiceProvider
        => _serviceProvider;

    public ApplicationServiceProvider()
    {
        Services = new ApplicationServiceCollection();
        _serviceProvider = Services.BuildServiceProvider();
    }

    protected override Type NormalizeServiceType(Type serviceType)
    {
        if (serviceType.IsGenericType && serviceType.ContainsGenericParameters)
        {
            var isEntityStateHandlerType = serviceType
                       .ImplementsGenericInterfaceDefinition(typeof(IEntityStateChangedEventHandler<>)) ||
                       serviceType.ImplementsGenericInterfaceDefinition(typeof(IEntityStateChangedEventPreProcessorHandler<>));

            if (isEntityStateHandlerType)
            {
                return serviceType.MakeGenericType(typeof(EntityBase));
            }

            var genericParameter = serviceType.GetGenericArguments()[0];
            if (genericParameter.IsGenericType && serviceType.ContainsGenericParameters)
            {
                var parameterNormalized = NormalizeServiceType(genericParameter);
                return serviceType.GetGenericTypeDefinition()
                    .MakeGenericType(parameterNormalized);
            }

            throw new InvalidOperationException($" The service {serviceType} has ContainsGenericParameters");

        }
        return serviceType;
    }

  
}
