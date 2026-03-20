using Snebur.UseCases;
using Snebur.ClientGateway;
using Microsoft.Extensions.DependencyInjection;

namespace Snebur.ArchitectureTests.TestSupport ;

public class ClientGatewayServiceCollection : ServiceCollection
{
    public ClientGatewayServiceCollection()
    {
        this.AddUserCasesSharedServices()
            .AddClientGatewayServices()
            .AddAdminUserAuthenticationServices()
            .AddTenantUserAuthenticationServices();

        this.AddSingleton<ITestOutputHelper, TestOutputProxy>();
    }
}
