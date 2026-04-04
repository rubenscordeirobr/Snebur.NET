namespace Snebur.Application.Abstractions.Mediators;

public interface IEventMediator
{
    Task PreProcessorDispatchAsync(
        IDomainEventContext eventContext,
        CancellationToken cancellationToken = default);

    Task DispatchAsync(IDomainEventContext eventContext, CancellationToken cancellationToken = default);
}
