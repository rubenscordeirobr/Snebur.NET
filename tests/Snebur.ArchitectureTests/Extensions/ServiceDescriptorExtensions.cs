using Microsoft.Extensions.DependencyInjection;

namespace Snebur.ArchitectureTests.Extensions;

public static class ServiceDescriptorExtensions
{
    public static bool IsAssignableTo(
        this ServiceDescriptor descriptor,
        IEnumerable<Type> targetTypes)
    {
        return descriptor.ServiceType?.IsAssignableTo(targetTypes) == true ||
                descriptor.ImplementationType?.IsAssignableTo(targetTypes) == true;
    }
}

