namespace Snebur.FunctionalTests.Identities;

public partial class TenantUserAuthenticationValidationServiceTests
{

    [Fact]
    public async Task EmailOrPhoneNumberExists_WithExistingEmail_ShouldReturnTrue()
    {
        //Arrange
        var email = SystemTenantConstants.Email;

        //Act
        var result = await _clientService.EmailOrPhoneNumberExitsAsync(
            email, cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        result.Should()
            .BeTrue();
    }

    [Fact]
    public async Task EmailOrPhoneNumberExists_WithExistingPhoneNumber_ShouldReturnTrue()
    {
        //Arrange
        var phoneNumber = SystemTenantConstants.PhoneNumber;

        //Act
        var result = await _clientService.EmailOrPhoneNumberExitsAsync(
            phoneNumber, cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        result.Should()
            .BeTrue();
    }

    [Fact]
    public async Task EmailOrPhoneNumberExists_WithExistingFormattedPhoneNumber_ShouldReturnTrue()
    {
        //Arrange
        var formattedPhoneNumber = BrazilianFormattingUtils.FormatPhone(
            SystemTenantConstants.PhoneNumber,
            internationalFormat: true);

        //Act
        var result = await _clientService.EmailOrPhoneNumberExitsAsync(
            formattedPhoneNumber, cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        result.Should()
            .BeTrue();
    }

    [Fact]
    public async Task EmailOrPhoneNumberExists_WithNonExistingEmail_ShouldReturnFalse()
    {
        //Arrange
        var email = FakeUtils.GenerateFakeEmail();

        //Act
        var result = await _clientService.EmailOrPhoneNumberExitsAsync(
            email, cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        result.Should()
            .BeFalse();
    }

    [Fact]
    public async Task EmailOrPhoneNumberExists_WithNonExistingPhoneNumber_ShouldReturnFalse()
    {
        //Arrange
        var phoneNumber = BrazilianFakeUtils.GenerateFakePhoneNumber();

        //Act
        var result = await _clientService.EmailOrPhoneNumberExitsAsync(
            phoneNumber, cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        result.Should()
            .BeFalse();
    }
}
