namespace Snebur.FunctionalTests.Identities;
public partial class TenantUserValidationServiceTests
    : IClassFixture<ClientServiceProviderMock<AnonymousRole>>
{
    private readonly ITenantUserValidationService _clientService;

    public TenantUserValidationServiceTests(
        ClientServiceProviderMock<AnonymousRole> serviceProvider,
        ITestOutputHelper testOutput)
    {
        serviceProvider.AddTestOutput(testOutput);

        _clientService = serviceProvider.GetRequiredService<ITenantUserValidationService>();
    }
}
