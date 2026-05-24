namespace Snebur.Core.UnitTests.Utils;

public class PhoneNumberValidationUtilsTests
{
    [Theory]
    [InlineData("+15551234567", true)]
    [InlineData("+521234567890", true)]
    [InlineData("+5511987654321", true)]
    [InlineData("123456", false)]
    [InlineData(null, false)]
    public void IsFullPhoneNumberValid_VariousNumbers_ReturnsExpectedValidity(
       string? fullNumber, bool expectedValidity)
    {
        var result = PhoneNumberValidationUtils.IsFullPhoneNumberValid(fullNumber);
        result.Should().Be(expectedValidity);
    }

    [Theory]
    [InlineData(Country.UnitedStates, "5551234567", true)]
    [InlineData(Country.Mexico, "1234567890", true)]
    [InlineData(Country.Brazil, "11987654321", true)]
    [InlineData(Country.Unknown, "123456", false)]
    public void IsNationalNumberValid_VariousNumbers_ReturnsExpectedValidity(Country countryCode, string nationalNumber, bool expectedValidity)
    {
        var result = PhoneNumberValidationUtils.IsNationalNumberValid(countryCode, nationalNumber);
        result.Should().Be(expectedValidity);
    }
}
