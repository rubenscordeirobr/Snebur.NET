using Snebur.SharedKernel.Models.Security;

namespace Snebur.ClientGateway.Abstractions;

public interface IClientTenantUserSessionContextService: IClientUserSessionContextService
{
    TenantUserSessionContext? SessionContext { get; }
    Task<TenantUserSessionContext?> GetSessionContextAsync();

    Task SetSessionContextAsync(TenantUserSessionContext sessionContext, bool isPersistent);
}

public sealed record TenantUserSessionContext(
        UserSessionClaims UserSessionClaims,
        UserSessionResponse UserSession,
        UserResponse User,
        TenantResponse Tenant);
