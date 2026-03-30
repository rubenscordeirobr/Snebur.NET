namespace Snebur.FunctionalTests.Identities;

public partial class TenantUserServiceTests
    : IClassFixture<ClientServiceProviderMock<TenantOwnerRole>>
{
    private readonly ITenantUserService _clientService;

    public TenantUserServiceTests(
        ClientServiceProviderMock<TenantOwnerRole> serviceProviderMock,
        ITestOutputHelper testOutput)
    {
        serviceProviderMock.AddTestOutput(testOutput);

        _clientService = serviceProviderMock.GetRequiredService<ITenantUserService>();
    }

}
