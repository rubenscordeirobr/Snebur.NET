using System.Dynamic;
using Snebur.Application.Abstractions.Persistence.Activities;
using Snebur.Domain.Entities.Activities;
using Snebur.Domain.Primitives;

namespace Snebur.UseCases.Activities.Events;

public sealed class EntityCreatedEventHandler<TEntity> : IEntityCreatedEventHandler<TEntity>
    where TEntity : EntityBase
{
    private readonly IActivityRepository _activityRepository;
    private readonly IHttpContextSessionAccessor _userSessionAccessor;
    private readonly ILogger<EntityCreatedEventHandler<TEntity>> _logger;

    public EntityCreatedEventHandler(
        IActivityRepository activityRepository,
        IHttpContextSessionAccessor userSessionAccessor,
        ILogger<EntityCreatedEventHandler<TEntity>> logger)
    {
        _activityRepository = activityRepository;
        _userSessionAccessor = userSessionAccessor;
        _logger = logger;
    }

    public async Task HandleAsync(IEntityCreatedEvent<TEntity> domainEvent,
        CancellationToken cancellationToken = default)
    {
        Guard.NotNull(domainEvent);

        var userSession = _userSessionAccessor.GetRequiredUserSession();
        var entity = domainEvent.Entity;

        var properties = domainEvent.PropertyValues
            .Select(propertyChanged => $"{propertyChanged.PropertyName}: {propertyChanged.Value}")
            .ToList();

        var description = $"Created {entity.GetType().Name} {entity.Id}. Properties: {string.Join(", ", properties)}";
        dynamic data = new ExpandoObject();
        foreach (var property in domainEvent.PropertyValues)
        {
            ((IDictionary<string, object>)data)[property.PropertyName] = property.Value ?? "null";
        }

        var dataSerialized = JsonUtils.Serialize(data);
        var qualifiedTypeName = entity.GetType().GetQualifiedName();

        var activity = new CreatedActivity
        {
            Tenant_Id = userSession.Tenant_Id,
            UserSession_Id = userSession.Id,
            ActivityDate = DateTime.UtcNow,
            Description = description,
            CreatedData = dataSerialized,
            Entity_Id = entity.Id,
            QualifiedTypeName = qualifiedTypeName,
        };

        try
        {
            await _activityRepository.AddAsync(activity, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on add activity for entity {EntityType} {EntityId}", entity.GetType().Name, entity.Id);
            throw;
        }
    }
}
