using Snebur.Application.Abstractions.Handlers;

namespace Snebur.Application.Events;

public sealed record ExecutedDomainEventResult(
    IDomainEvent DomainEvent,
    Type HandlerType,
    Type? ImplementationHandlerType,
    IApplicationHandler? Handler,
    Exception? Exception)
{
    public bool IsSuccess 
        => Exception is null;
}
