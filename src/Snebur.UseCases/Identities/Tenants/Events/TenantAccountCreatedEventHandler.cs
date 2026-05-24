using Snebur.Domain.Entities.Identities.Events;

namespace Snebur.UseCases.Identities.Tenants.Events;

internal class TenantAccountCreatedEventHandler : IDomainEventHandler<TenantCreatedEvent>
{
    private readonly ILogger<TenantAccountCreatedEventHandler> _logger;
 
    public TenantAccountCreatedEventHandler(
        ILogger<TenantAccountCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(TenantCreatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Tenant created: {Tenant}", domainEvent.Tenant);

        //TODO : Send email to owner and Create user session for owner to avoid login again
        return Task.CompletedTask;
    }
}
