using Snebur.Application.Abstractions.Services;
using Snebur.Testing.Core.Mocks.Infrastructure;

namespace Snebur.FunctionalTests.Identities;

 
public partial class TenantUserAuthenticationServiceTests
    : IClassFixture<ClientServiceProviderMock<AnonymousRole>>
{
    private readonly ITenantUserAuthenticationService _clientService;
    private readonly IServiceProvider _hostServiceProvider;

    public TenantUserAuthenticationServiceTests(
        ClientServiceProviderMock<AnonymousRole> serviceProviderMock,
        ITestOutputHelper testOutput)
    {
        serviceProviderMock.AddTestOutput(testOutput);

        _clientService = serviceProviderMock.GetRequiredService<ITenantUserAuthenticationService>();
        _hostServiceProvider = serviceProviderMock.HostServiceProvider;
    }
     
    private void ClearCache()
    {
        var cacheRepository = (CacheRepositoryMock)_hostServiceProvider.GetRequiredService<ICacheRepository>();
        cacheRepository.Clear("authentication-attempt-limiter");
    }

}
