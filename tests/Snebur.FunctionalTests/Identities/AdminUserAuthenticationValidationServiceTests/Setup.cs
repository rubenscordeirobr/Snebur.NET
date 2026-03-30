namespace Snebur.FunctionalTests.Identities;

public partial class AdminUserAuthenticationValidationServiceTests
    : IClassFixture<ClientServiceProviderMock<AnonymousRole>>
{
    private readonly IAdminUserAuthenticationValidationService _clientService;

    public AdminUserAuthenticationValidationServiceTests(
        ClientServiceProviderMock<AnonymousRole> serviceProviderMock,
        ITestOutputHelper testOutput)
    {
        serviceProviderMock.AddTestOutput(testOutput);

        _clientService = serviceProviderMock.GetRequiredService<IAdminUserAuthenticationValidationService>();
    }

}
