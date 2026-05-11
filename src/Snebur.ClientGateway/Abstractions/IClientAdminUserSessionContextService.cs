using Snebur.SharedKernel.Models.Security;

namespace Snebur.ClientGateway.Abstractions;

public interface IClientAdminUserSessionContextService : IClientUserSessionContextService
{
    AdminUserSessionContext? SessionContext { get; }
    Task<AdminUserSessionContext?> GetSessionContextAsync(); 

    Task SetSessionContextAsync(AdminUserSessionContext sessionContext, bool isPersistent);
}

public sealed record AdminUserSessionContext(
        UserSessionClaims UserSessionClaims,
        UserSessionResponse UserSession,
        UserResponse user);
