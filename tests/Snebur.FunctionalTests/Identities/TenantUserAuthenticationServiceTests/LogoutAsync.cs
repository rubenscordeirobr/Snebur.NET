using Snebur.ClientGateway.Abstractions;
using Snebur.UseCases.Identities.Authentications.Commands;

namespace Snebur.FunctionalTests.Identities;

public partial class TenantUserAuthenticationServiceTests
{
    [Fact]
    public async Task LogoutAsync_WhenAnonymousUser_ShouldBeFailure()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var command = new TenantUserLogoutCommand(sessionId);

        // Act
        var result = await _clientService.LogoutAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeFailureForErrors<ForbiddenError, AuthenticationError>();
    }

    public class TheLogoutAsyncMethod_WithTenantOwnerLogged
        : IClassFixture<ClientServiceProviderMock<TenantOwnerRole>>
    {
        private readonly ITenantUserAuthenticationService _clientService;
        private readonly IServiceProvider _clientServiceProvider;

        public TheLogoutAsyncMethod_WithTenantOwnerLogged(
            ClientServiceProviderMock<TenantOwnerRole> serviceProviderMock,
            ITestOutputHelper testOutput)
        {
            serviceProviderMock.AddTestOutput(testOutput);

            _clientServiceProvider = serviceProviderMock; ;
            _clientService = serviceProviderMock.GetRequiredService<ITenantUserAuthenticationService>();
        }

        [Fact]
        public async Task LogoutAsync_WhenValidCredentials_ShouldBeSuccessful()
        {
            // Arrange
            var userSessionService = _clientServiceProvider.GetRequiredService<IClientTenantUserSessionContextService>();
            var userSessionContext = userSessionService.SessionContext;

            var command = new TenantUserLogoutCommand(userSessionContext!.UserSession.Id);

            //// Act
            var result = await _clientService.LogoutAsync(command, cancellationToken: TestContext.Current.CancellationToken);

            //// Assert
            result.ShouldBeSuccessful();
        }

        [Fact]
        public async Task LogoutAsync_WhenInvalidSessionId_ShouldBeFailure()
        {
            // Arrange
            var sessionId = Guid.NewGuid();
            var command = new TenantUserLogoutCommand(sessionId);

            // Act
            var result = await _clientService.LogoutAsync(command, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.ShouldBeFailureForErrors<ForbiddenError, AuthenticationError>();
        }
    }
}
