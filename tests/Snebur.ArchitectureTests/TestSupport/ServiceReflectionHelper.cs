using Snebur.Application.Abstractions.Handlers;
using Snebur.Application.Abstractions.Persistence;
using Snebur.Application.Abstractions.Services;
using Snebur.Presentation.Common;
using Snebur.SharedKernel.ValueObjects;

namespace Snebur.ArchitectureTests.TestSupport;

public static class ServiceReflectionHelper
{
    private static readonly Type[] ServiceContractInterfaceTypes = [
        typeof(IRepositoryBase<>),
            typeof(IApplicationHandler),
            typeof(IValidator),
            typeof(IValidationService),
            typeof(IApplicationService)
    ];

    public static bool IsImplementService(Type type)
    {
        if (type.ImplementsGenericInterfaceDefinition(typeof(IValidator<>)))
        {
            var validationItemType = type.GetGenericArgumentFromInterfaceDefinition(typeof(IValidator<>));
            if (validationItemType.IsSubclassOf<ValueObjectBase>())
            {
                return false;
            }
        }

        return !type.IsSubclassOf<ApiEndpointBase>()
            && type.IsConcrete()
            && type.IsAssignableTo(ServiceContractInterfaceTypes);
    }
}
