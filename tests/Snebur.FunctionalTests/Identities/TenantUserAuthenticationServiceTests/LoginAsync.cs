using Snebur.UseCases.Identities.Authentications.Commands;

namespace Snebur.FunctionalTests.Identities;

public partial class TenantUserAuthenticationServiceTests
{
    [Fact]
    public async Task LoginAsync_WhenValidCredentials_ShouldBeSuccessful()
    {
        // Arrange
         
        ClearCache(); // To avoid the TooManyRequestsError

        var email = SystemTenantConstants.Email;
        var testPassword = SystemTenantConstants.TestPassword;

        var command = new TenantUserLoginCommand
        {
            EmailOrPhoneNumber = email,
            Password = testPassword,
            IsPersistent = true
        };

        // Act
        var result = await _clientService.LoginAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeSuccessful();
    }

    [Fact]
    public async Task LoginAsync_WhenInvalidEmailCredentials_ShouldBeFailure()
    {
        // Arrange
        var invalidEmail = FakeUtils.GenerateFakeEmail();
        var testPassword = SystemTenantConstants.TestPassword;

        var command = new TenantUserLoginCommand
        {
            EmailOrPhoneNumber = invalidEmail,
            Password = testPassword,
            IsPersistent = true
        };

        // Act
        var result = await _clientService.LoginAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeFailure<ValidationError>();
    }

    [Fact]
    public async Task LoginAsync_WhenInvalidPasswordCredentials_ShouldBeFailure()
    {
        ClearCache(); // To avoid the TooManyRequestsError

        // Arrange
        var email = SystemTenantConstants.Email;
        var invalidPassword = "invalidPassword";

        var command = new TenantUserLoginCommand
        {
            EmailOrPhoneNumber = email,
            Password = invalidPassword,
            IsPersistent = true
        };

        // Act
        var result = await _clientService.LoginAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeFailure<AuthenticationError>();
    }

    [Fact]
    public async Task LoginAsync_WhenManyRequests_ShouldBeFailure()
    {
        // Arrange
        var email = SystemTenantConstants.Email;
        var invalidPassword = "invalidPassword";

        var command = new TenantUserLoginCommand
        {
            EmailOrPhoneNumber = email,
            Password = invalidPassword,
            IsPersistent = true
        };

        // Act
        var attempt1 = await _clientService.LoginAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        var attempt2 = await _clientService.LoginAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        var attempt3 = await _clientService.LoginAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        var attempt4 = await _clientService.LoginAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        var attempt5 = await _clientService.LoginAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        var result = await _clientService.LoginAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        attempt1.ShouldBeFailureForErrors<AuthenticationError, TooManyRequestsError>(isAssignableTo: true);
        attempt2.ShouldBeFailureForErrors<AuthenticationError, TooManyRequestsError>(isAssignableTo: true);
        attempt3.ShouldBeFailureForErrors<AuthenticationError, TooManyRequestsError>(isAssignableTo: true);
        attempt4.ShouldBeFailureForErrors<AuthenticationError, TooManyRequestsError>(isAssignableTo: true);
        attempt5.ShouldBeFailureForErrors<AuthenticationError, TooManyRequestsError>(isAssignableTo: true);

        result.ShouldBeFailure<TooManyRequestsError>();

        ClearCache();
    }
}

