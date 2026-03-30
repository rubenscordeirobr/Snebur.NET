namespace Snebur.FunctionalTests.Identities;

public partial class AdminUserServiceTests 
    : IClassFixture<ClientServiceProviderMock<AdminUserRole>>
{
    private readonly IAdminUserService _clientService;

    public AdminUserServiceTests(
        ClientServiceProviderMock<AdminUserRole> serviceProviderMock,
        ITestOutputHelper testOutput)
    {
        serviceProviderMock.AddTestOutput(testOutput);

        _clientService = serviceProviderMock.GetRequiredService<IAdminUserService>();
    }

}
