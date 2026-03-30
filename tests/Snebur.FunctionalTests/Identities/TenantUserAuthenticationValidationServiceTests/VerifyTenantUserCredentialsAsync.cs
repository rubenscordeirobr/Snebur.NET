namespace Snebur.FunctionalTests.Identities;

public partial class TenantUserAuthenticationValidationServiceTests
{

    [Fact]
    public async Task VerifyTenantUserCredentials_WithValidEmailAndPassword_ShouldReturnTrue()
    {
        // Arrange
        var email = SystemTenantConstants.Email;
        var testPassword = SystemTenantConstants.TestPassword;
        
        // Act
        var result = await _clientService.VerifyTenantUserCredentialsAsync(
            email, 
            testPassword,
            cancellationToken: TestContext.Current.CancellationToken);
        
        // Assert
        result.Should()
            .BeTrue();              
    }

    [Fact]
    public async Task VerifyTenantUserCredentials_WithValidPhoneNumberAndPassword_ShouldReturnTrue()
    {
        // Arrange
        var phoneNumber = SystemTenantConstants.PhoneNumber;
        var testPassword = SystemTenantConstants.TestPassword;
      
        // Act
        var result = await _clientService.VerifyTenantUserCredentialsAsync(
            phoneNumber,
            testPassword,
            cancellationToken: TestContext.Current.CancellationToken);
        
        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task VerifyTenantUserCredentials_WithValidFormattedPhoneNumberAndPassword_ShouldReturnTrue()
    {
        // Arrange
        var formattedPhoneNumber = BrazilianFormattingUtils.FormatPhone(
            SystemTenantConstants.PhoneNumber,
            internationalFormat: true);
        
        var testPassword = SystemTenantConstants.TestPassword;

        // Act
        var result = await _clientService.VerifyTenantUserCredentialsAsync(
            formattedPhoneNumber,
            testPassword, 
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task VerifyTenantUserCredentials_WithValidEmailAndInvalidPassword_ShouldReturnFalse()
    {
        // Arrange
        var email = SystemTenantConstants.Email;
        var testPassword = "invalid";

        // Act
        var result = await _clientService.VerifyTenantUserCredentialsAsync(
            email, 
            testPassword,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.Should()
            .BeFalse();
    }

    [Fact]
    public async Task VerifyTenantUserCredentials_WithValidPhoneNumberAndInvalidPassword_ShouldReturnFalse()
    {
        // Arrange
        var phoneNumber = SystemTenantConstants.PhoneNumber;
        var testPassword = "invalid";

        // Act
        var result = await _clientService.VerifyTenantUserCredentialsAsync(phoneNumber, testPassword, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.Should()
            .BeFalse();
    }

    [Fact]
    public async Task VerifyTenantUserCredentials_WithInvalidEmail_ShouldReturnFalse()
    {
        // Arrange
        var email = FakeUtils.GenerateFakeEmail();
        var testPassword = SystemTenantConstants.TestPassword;
        // Act
        var result = await _clientService.VerifyTenantUserCredentialsAsync(email, testPassword, cancellationToken: TestContext.Current.CancellationToken);
        // Assert
        result.Should()
            .BeFalse();
    }

    [Fact]
    public async Task VerifyTenantUserCredentials_WithInvalidPhoneNumber_ShouldReturnFalse()
    {
        // Arrange
        var phoneNumber = BrazilianFakeUtils.GenerateFakePhoneNumber();
        var testPassword = SystemTenantConstants.TestPassword;
        // Act
        var result = await _clientService.VerifyTenantUserCredentialsAsync(phoneNumber, testPassword, cancellationToken: TestContext.Current.CancellationToken);
        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task VerifyTenantUserCredentials_WithInvalidEmailAndInvalidPassword_ShouldReturnFalse()
    {
        // Arrange
        var email = FakeUtils.GenerateFakeEmail();
        var testPassword = "invalid";
        // Act
        var result = await _clientService.VerifyTenantUserCredentialsAsync(email, testPassword, cancellationToken: TestContext.Current.CancellationToken);
        // Assert
        result.Should()
            .BeFalse();
    }

    [Fact]
    public async Task VerifyTenantUserCredentials_WithInvalidPhoneNumberAndInvalidPassword_ShouldReturnFalse()
    {
        // Arrange
        var phoneNumber = BrazilianFakeUtils.GenerateFakePhoneNumber();
        var testPassword = "invalid";
        // Act
        var result = await _clientService.VerifyTenantUserCredentialsAsync(phoneNumber, testPassword, cancellationToken: TestContext.Current.CancellationToken);
        // Assert
        result.Should().BeFalse();
    }

}

