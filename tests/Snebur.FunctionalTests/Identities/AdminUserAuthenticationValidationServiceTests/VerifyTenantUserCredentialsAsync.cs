namespace Snebur.FunctionalTests.Identities;

public partial class AdminUserAuthenticationValidationServiceTests
{

    [Fact]
    public async Task VerifyAdminUserCredentials_WithValidEmailAndPassword_ShouldReturnTrue()
    {
        // Arrange
        var email = DefaultAdminUserConstants.Email;
        var testPassword = DefaultAdminUserConstants.TestPassword;
        
        // Act
        var result = await _clientService.VerifyAdminUserCredentialsAsync(
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
        var phoneNumber = DefaultAdminUserConstants.PhoneNumber;
        var testPassword = DefaultAdminUserConstants.TestPassword;
      
        // Act
        var result = await _clientService.VerifyAdminUserCredentialsAsync(
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
            DefaultAdminUserConstants.PhoneNumber,
            internationalFormat: true);
        
        var testPassword = DefaultAdminUserConstants.TestPassword;

        // Act
        var result = await _clientService.VerifyAdminUserCredentialsAsync(
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
        var email = DefaultAdminUserConstants.Email;
        var testPassword = "invalid";

        // Act
        var result = await _clientService.VerifyAdminUserCredentialsAsync(
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
        var phoneNumber = DefaultAdminUserConstants.PhoneNumber;
        var testPassword = "invalid";

        // Act
        var result = await _clientService.VerifyAdminUserCredentialsAsync(
            phoneNumber, 
            testPassword,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.Should()
            .BeFalse();
    }

    [Fact]
    public async Task VerifyTenantUserCredentials_WithInvalidEmail_ShouldReturnFalse()
    {
        // Arrange
        var email = FakeUtils.GenerateFakeEmail();
        var testPassword = DefaultAdminUserConstants.TestPassword;
        // Act
        var result = await _clientService.VerifyAdminUserCredentialsAsync(email, testPassword, cancellationToken: TestContext.Current.CancellationToken);
        // Assert
        result.Should()
            .BeFalse();
    }

    [Fact]
    public async Task VerifyTenantUserCredentials_WithInvalidPhoneNumber_ShouldReturnFalse()
    {
        // Arrange
        var phoneNumber = BrazilianFakeUtils.GenerateFakePhoneNumber();
        var testPassword = DefaultAdminUserConstants.TestPassword;
        // Act
        var result = await _clientService.VerifyAdminUserCredentialsAsync(phoneNumber, testPassword, cancellationToken: TestContext.Current.CancellationToken);
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
        var result = await _clientService.VerifyAdminUserCredentialsAsync(email, testPassword, cancellationToken: TestContext.Current.CancellationToken);
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
        var result = await _clientService.VerifyAdminUserCredentialsAsync(phoneNumber, testPassword, cancellationToken: TestContext.Current.CancellationToken);
        // Assert
        result.Should().BeFalse();
    }

}

