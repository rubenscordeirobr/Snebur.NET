using Snebur.Application.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace Snebur.Application.Registrars;

internal partial class ApplicationHandlerRegistrar
{
    private static class ValidatorHelper
    {
        public static void EnsureNoOtherHandlerRegistered(
            IServiceCollection services,
            Type serviceType,
            Type handlerType,
            Type requestType,
            Type responseType)
        {
            var registeredServices = services.Where(x => x.ServiceType == serviceType && x.ImplementationType != handlerType);
            if (registeredServices.Any())
            {
                var errorMessage = GetHandlerAlreadyRegisteredErrorMessage(
                    registeredServices,
                    handlerType,
                    requestType,
                    responseType);

                throw new HandlerRegistrationAlreadyExistsException(errorMessage);
            }
        }

        public static string GetHandlerAlreadyRegisteredErrorMessage(
            IEnumerable<ServiceDescriptor> registeredServices,
            Type handlerType,
            Type requestType,
            Type responseType)
        {
            var implementationType = registeredServices.First().ImplementationType!;
            return $"The handler type {handlerType.GetQualifiedName()} can't be registrad for " +
                   $"the request type {requestType.GetQualifiedName()} and response type {responseType.GetQualifiedName()} " +
                   $"because there is already a handler registrad for this request type and response type. " +
                   $"The handler {implementationType.GetQualifiedName()} is already registrad for this request type and response type";
        }

        public static void CheckIfClass(Type handlerType)
        {
            if (!handlerType.IsClass)
            {
                var message = $"Handler {handlerType.Name} is not a class";
                throw new InvalidOperationException(message);
            }
        }

        public static void CheckIfHasSubclassIn(Type handlerType, Type[] assemblyTypes)
        {
            if (handlerType.IsSealed)
                return;

            var subClass = assemblyTypes.FirstOrDefault(x => x.IsSubclassOf(handlerType));
            if (subClass is not null)
            {
                var message = $"Handler {handlerType.Name} has a subclass  {subClass} in the assembly" +
                              "Please, make sure that the handler is not being used as a base class for another or set the handler as abstract class";
                throw new InvalidOperationException(message);
            }
        }

        public static void CheckIfNotGenericType(Type type)
        {
            if (type.IsGenericType)
            {
                var message = $"Type {type.Name} is a generic type";
                throw new InvalidOperationException(message);
            }
        }

        public static void ValidateDomainEventType(Type eventHandlerType)
        {
            if (eventHandlerType.IsGenericType)
            {
                if (eventHandlerType.GetGenericArguments().Length > 1)
                {
                    var message = $"Event {eventHandlerType.Name} has more than one generic argument";
                    throw new NotSupportedException(message);
                }

                if (!eventHandlerType.ImplementsGenericInterfaceDefinition(typeof(IEntityStateChangedEvent<>)))
                {
                    var message = $"The generic event {eventHandlerType.Name} does not implement IEntityStateChangedEvent<>";
                    throw new NotSupportedException(message);
                }
            }
        }

        public static void ValidateHandlerType(
            Type handlerType,
            HandlerKind eventHandlerType)
        {
            Guard.NotNull(handlerType);

            if (!handlerType.IsClass)
            {
                var message = $"Handler {handlerType.Name} is not a class";
                throw new InvalidOperationException(message);
            }

            if (!handlerType.IsGenericType)
            {
                return;
            }

            if (handlerType.GetGenericArguments().Length > 1)
            {
                var message = $"Handler {handlerType.Name} has more than one generic argument";
                throw new NotSupportedException(message);
            }

            var genericArguments = handlerType.GetGenericArguments()[0];
            if (!genericArguments.IsAssignableTo(typeof(IDomainEvent)) &&
                !genericArguments.IsSubclassOfOrEquals<EntityBase>())
            {
                var message = $"The generic handler {handlerType.Name} does not implement IDomainEvent";
                throw new NotSupportedException(message);
            }

            if (genericArguments.IsGenericType)
            {
                throw new NotSupportedException(
                    $" The generic handler {handlerType.Name} has a generic argument that is also a generic type that is not supported");
            }

            if (genericArguments.IsSubclassOfOrEquals<EntityBase>())
            {
                if (eventHandlerType == HandlerKind.DomainEventHandler &&
                    !handlerType.ImplementsGenericInterfaceDefinition(typeof(IEntityStateChangedEventHandler<>)))
                {
                    var message = $"The generic handler {handlerType.Name} does not implement IEntityStateChangedEventHandler<>";
                    throw new NotSupportedException(message);

                }

                if (eventHandlerType == HandlerKind.PreProcessorHandler &&
                    !handlerType.ImplementsGenericInterfaceDefinition(typeof(IEntityStateChangedEventPreProcessorHandler<>)))
                {
                    var message = $"The generic handler {handlerType.Name} does not implement IEntityStateChangedEventPreProcessor<>";
                    throw new NotSupportedException(message);
                }
            }
        }
    }
}
