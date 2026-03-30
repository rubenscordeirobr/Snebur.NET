namespace Snebur.FunctionalTests.Identities;

public partial class TenantUserValidationServiceTests
{
    [Fact]
    public async Task IsPhoneNumberUnique_ShouldReturnTrue()
    {
        var phoneNumber = BrazilianFakeUtils.GenerateFakePhoneNumber();

        var result = await _clientService.IsPhoneNumberUniqueAsync(
            phoneNumber,
            cancellationToken: TestContext.Current.CancellationToken);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsPhoneNumberUnique_ShouldReturnFalse()
    {
        var phoneNumber = SystemTenantConstants.PhoneNumber;

        var result = await _clientService.IsPhoneNumberUniqueAsync(
            phoneNumber,
            cancellationToken: TestContext.Current.CancellationToken);

        result.Should()
            .BeFalse();
    }

    [Fact]
    public async Task IsPhoneNumberUnique_WithFormattedNumber_ShouldReturnFalse()
    {
        var phoneNumber = SystemTenantConstants.PhoneNumber;
        var phoneNumberFormatted = BrazilianFormattingUtils.FormatPhone(
            phoneNumber, 
            internationalFormat: true);

        var result = await _clientService.IsPhoneNumberUniqueAsync(
            phoneNumberFormatted,
            cancellationToken: TestContext.Current.CancellationToken);

        result.Should()
            .BeFalse();
    }

    [Fact]
    public async Task IsPhoneNumberUnique_WithTenantAndOwner_ShouldReturnTrue()
    {
        var phoneNumber = BrazilianFakeUtils.GenerateFakePhoneNumber();

        var result = await _clientService.IsPhoneNumberUniqueAsync(
            SystemTenantConstants.User_Id,
            phoneNumber,
            cancellationToken: TestContext.Current.CancellationToken);

        result.Should().BeTrue();
    }
}
