namespace Snebur.Application.Extensions;

public static class EntityDeletedExtensions
{
    public static void MarkAsDeleted(
        this ISoftDeletableEntity entity,
        IUserSession userSession)
    {
        Guard.NotNull(entity);
        Guard.NotNull(userSession);

        if (userSession.IsAnonymous())
        {
            throw new ForbiddenSecurityException("Cannot delete entity with anonymous session");
        }

        if (userSession.IsTenantUser() &&
            entity is ITenantOwned entityTenant &&
            entityTenant.Tenant_Id != userSession.Tenant_Id)
        {
            throw new ForbiddenSecurityException("Cannot delete entity from another tenant");
        }

        var entityType = entity.GetType();
        var properties = entityType.GetPropertiesFromInterface<ISoftDeletableEntity>();

        properties[nameof(ISoftDeletableEntity.DeletedAt)]
            .SetValue(entity, DateTime.UtcNow);

        properties[nameof(ISoftDeletableEntity.DeletedSession_Id)]
            .SetValue(entity, userSession.Id);

        properties[nameof(ISoftDeletableEntity.IsDeleted)]
            .SetValue(entity, true);
    }
}
