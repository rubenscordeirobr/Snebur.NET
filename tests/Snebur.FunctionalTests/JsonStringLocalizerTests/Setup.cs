using Snebur.SharedKernel.Abstractions;

namespace Snebur.FunctionalTests;

public partial class JsonStringLocalizerServiceTests : IClassFixture<ClientServiceProviderMock<AnonymousRole>>
{
    private readonly HttpClient _httpClient;
    private readonly IJsonStringLocalizerService _clientService;

    public JsonStringLocalizerServiceTests(
        ClientServiceProviderMock<AnonymousRole> serviceProvider,
        ITestOutputHelper testOutput)
    {
        serviceProvider.AddTestOutput(testOutput);

        _clientService = serviceProvider.GetRequiredService<IJsonStringLocalizerService>();
        _httpClient = serviceProvider.CreateHttpClientProvider().GetHttpClient<IJsonStringLocalizerService>();
    }
}
