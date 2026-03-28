using Snebur.Application.Extensions;

namespace Snebur.RuntimeServices.Services;

public class EntityAuthorizationService : IEntityAuthorizationService
{
    public void ValidateEntityChange(
         EntityBase entity,
         IUserSession userSession,
         EntityChangeState entityChangeState)
    {
        Guard.NotNull(entity);
        Guard.NotNull(userSession);

        ValidateAuthorization(entity, userSession, entityChangeState);
        ValidateRole(entity, userSession, entityChangeState);
    }

    private void ValidateAuthorization(
        EntityBase entity,
        IUserSession userSession,
        EntityChangeState entityChangeState)
    {
        if (HasAuthorization(entity, userSession, entityChangeState))
        {
            return;
        }

        throw new ForbiddenSecurityException(
            $"Access Denied: User {userSession.User_Id} (Role: {userSession.UserRole}, Tenant_Id: {userSession.Tenant_Id}) " +
            $"is not authorized to perform '{entityChangeState}' on entity '{entity.GetType().Name}' (ID: {entity.Id}, Tenant_Id: {(entity as ITenantOwned)?.Tenant_Id}).");
    }

    private bool HasAuthorization(
        EntityBase entity,
        IUserSession userSession,
        EntityChangeState entityChangeState)
    {
        if (userSession.IsAnonymous())
        {
            return CheckAnonymousPermission(entity, userSession, entityChangeState);
        }

        if (entity is UserSession &&
            entity.Id == userSession.Id)
        {
            return true;
        }

        if (userSession.IsTenantUser())
        {
            if (entity is ITenantOwned entityTenant)
            {
                return entityTenant.Tenant_Id == userSession.Tenant_Id;
            }
            return false;
        }
        return userSession.IsSystemAdminUser();
    }

    private bool CheckAnonymousPermission(
        EntityBase entity,
        IUserSession userSession,
        EntityChangeState entityChangeState)
    {
        if (entityChangeState == EntityChangeState.Created)
        {
            return entity is Tenant or TenantUser or UserSession;
        }

        if (entity is UserSession)
        {
            return userSession.Id == AnonymousUserConstants.Session_Id;
        }

        if (entityChangeState == EntityChangeState.Updated)
        {
            return entity.CreatedSession_Id == userSession.Id;
        }
         
        return false;

    
    }

    private void ValidateRole(
        EntityBase entity,
        IUserSession userSession,
        EntityChangeState state)
    {
        //not implement yet
    }
}
