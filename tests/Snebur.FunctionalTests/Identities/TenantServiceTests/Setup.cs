namespace Snebur.FunctionalTests.Identities;

public partial class TenantServiceTests
    : IClassFixture<ClientServiceProviderMock<AdminUserRole>>
{
    private readonly ITenantService _clientService;

    public TenantServiceTests(
        ClientServiceProviderMock<AdminUserRole> serviceProvider,
        ITestOutputHelper testOutput)
    {
        serviceProvider.AddTestOutput(testOutput);

        _clientService = serviceProvider.GetRequiredService<ITenantService>();
    }
}
