namespace Snebur.FunctionalTests.Identities;

public partial class TenantUserAuthenticationValidationServiceTests
    : IClassFixture<ClientServiceProviderMock<AnonymousRole>>
{
    private readonly ITenantUserAuthenticationValidationService _clientService;

    public TenantUserAuthenticationValidationServiceTests(
        ClientServiceProviderMock<AnonymousRole> serviceProviderMock,
        ITestOutputHelper testOutput)
    {
        serviceProviderMock.AddTestOutput(testOutput);

        _clientService = serviceProviderMock.GetRequiredService<ITenantUserAuthenticationValidationService>();
    }

}
