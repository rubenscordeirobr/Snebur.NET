using Snebur.ClientGateway.Abstractions;

namespace Snebur.FunctionalTests.Mocks;

public class ClientTenantUserSessionContextMock : IClientTenantUserSessionContextService
{
    public TenantUserSessionContext? SessionContext { get; private set; }

    public Task<TenantUserSessionContext?> GetSessionContextAsync()
    {
        return Task.FromResult(SessionContext);
    }

    public Task SetSessionContextAsync(
        TenantUserSessionContext sessionContext,
        bool isPersistent)
    {
        SessionContext = sessionContext;
        return Task.CompletedTask;
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public Task ClearSessionContextAsync()
    {
        SessionContext = null;
        return Task.CompletedTask;
    }
}
