namespace Snebur.FunctionalTests.Identities;
public partial class TenantValidationServiceTests 
    : IClassFixture<ClientServiceProviderMock<AnonymousRole>>
{
    private readonly ITenantValidationService _clientService;

    public TenantValidationServiceTests(
        ClientServiceProviderMock<AnonymousRole> serviceProvider,
        ITestOutputHelper testOutput)
    {
        serviceProvider.AddTestOutput(testOutput);

        _clientService = serviceProvider.GetRequiredService<ITenantValidationService>();
    }    
}
