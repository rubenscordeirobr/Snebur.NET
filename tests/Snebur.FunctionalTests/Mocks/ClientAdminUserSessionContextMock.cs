using Snebur.ClientGateway.Abstractions;

namespace Snebur.FunctionalTests.Mocks;

public class ClientAdminUserSessionContextMock : IClientAdminUserSessionContextService
{
    public AdminUserSessionContext? SessionContext { get; private set; }

    public Task<AdminUserSessionContext?> GetSessionContextAsync()
    {
        return Task.FromResult(SessionContext);
    }

    public Task SetSessionContextAsync(AdminUserSessionContext sessionContext, bool isPersistent)
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
