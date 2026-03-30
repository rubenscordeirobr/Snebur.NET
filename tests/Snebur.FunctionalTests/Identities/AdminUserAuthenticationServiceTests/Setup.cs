using Snebur.Application.Abstractions.Services;
using Snebur.Testing.Core.Mocks.Infrastructure;

namespace Snebur.FunctionalTests.Identities;

 
public partial class AdminUserAuthenticationServiceTests
    : IClassFixture<ClientServiceProviderMock<AnonymousRole>>
{
    private readonly IAdminUserAuthenticationService _clientService;
    private readonly IServiceProvider _hostServiceProvider;

    public AdminUserAuthenticationServiceTests(
        ClientServiceProviderMock<AnonymousRole> serviceProviderMock,
        ITestOutputHelper testOutput)
    {
        serviceProviderMock.AddTestOutput(testOutput);

        _clientService = serviceProviderMock.GetRequiredService<IAdminUserAuthenticationService>();
        _hostServiceProvider = serviceProviderMock.HostServiceProvider;
    }
     
    private void ClearCache()
    {
        var cacheRepository = (CacheRepositoryMock)_hostServiceProvider.GetRequiredService<ICacheRepository>();
        cacheRepository.Clear("authentication-attempt-limiter");
    }

}
