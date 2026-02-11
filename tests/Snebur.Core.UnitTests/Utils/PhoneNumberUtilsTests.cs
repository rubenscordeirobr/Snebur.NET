namespace Snebur.Core.UnitTests.Utils;

public class PhoneNumberUtilsTests
{
    [Theory]
    [InlineData("+15551234567", Country.UnitedStates)]
    [InlineData("+521234567890", Country.Mexico)]
    [InlineData("+5511987654321", Country.Brazil)]
    public void GetCountryCode_ValidNumber_ReturnsExpectedCountryCode(string fullNumber, Country expectedCountryCode)
    {
        var result = PhoneNumberUtils.GetCountryCode(fullNumber);

        result.Should().Be(expectedCountryCode);
    }

    [Theory]
    [InlineData("+15551234567", InternationalDialingCode.UnitedStates)]
    [InlineData("+521234567890", InternationalDialingCode.Mexico)]
    [InlineData("+5511987654321", InternationalDialingCode.Brazil)]
    public void GetInternationalDialingCode_ValidNumber_ReturnsExpectedDialingCode(string fullNumber, InternationalDialingCode expectedDialingCode)
    {
        var result = PhoneNumberUtils.GetInternationalDialingCode(fullNumber);
        result.Should().Be(expectedDialingCode);
    } 

    [Theory]
    [InlineData(Country.UnitedStates, "5551234567", "+15551234567")]
    [InlineData(Country.Mexico, "1234567890", "+521234567890")]
    [InlineData(Country.Brazil, "11987654321", "+5511987654321")]
    public void GetFullPhoneNumber_ValidInputs_ReturnsExpectedFullNumber(
        Country country, string nationalNumber, string expectedFullNumber)
    {
        var result = PhoneNumberUtils.GetFullPhoneNumber(country, nationalNumber);

        result.Should().Be(expectedFullNumber);
    }

    [Theory]
    [InlineData(Country.UnitedStates, "+15551234567", "+15551234567")]
    [InlineData(Country.Mexico, "+521234567890", "+521234567890")]
    [InlineData(Country.Brazil, "+5511987654321", "+5511987654321")]
#pragma warning disable S4144  
    public void GetFullPhoneNumber_WithExistingInternationalFormat_ReturnsUnchanged(

        Country country, string nationalNumber, string expectedFullNumber)
    {
        var result = PhoneNumberUtils.GetFullPhoneNumber(country, nationalNumber);

        result.Should().Be(expectedFullNumber);
    }
#pragma warning restore S4144

    [Theory]
    [InlineData(Country.UnitedStates, "+25551234567")]
    [InlineData(Country.Mexico, "+31234567890")]
    [InlineData(Country.Brazil, "+111987654321")]
    public void GetFullPhoneNumber_WrongInternationalDialingCode_ThrowsArgumentException(
        Country country, string nationalNumber)
    {
        var action = () => PhoneNumberUtils.GetFullPhoneNumber(country, nationalNumber);

        action.Should().Throw<ArgumentException>()
            .WithMessage("*contains invalid international dialing code*")
            .And.ParamName.Should().Be("nationalNumber");
    }

    [Theory]
    [InlineData(Country.UnitedStates, "123")]
    [InlineData(Country.Mexico, "456")]
    [InlineData(Country.Brazil, "789")]
    public void GetFullPhoneNumber_NationalNumberTooShort_ThrowsArgumentException(
        Country country, string nationalNumber)
    {
        var action = () => PhoneNumberUtils.GetFullPhoneNumber(country, nationalNumber);

        action.Should().Throw<ArgumentException>()
            .WithMessage("*is too short*")
            .And.ParamName.Should().Be("nationalNumber");
    }

    [Fact]
    public void GetFullPhoneNumber_UnsupportedCountryCode_ThrowsArgumentException()
    {
        var action = () => PhoneNumberUtils.GetFullPhoneNumber(Country.Unknown, "1234567890");

        action.Should().Throw<ArgumentException>()
            .WithMessage("*is not supported for phone number formatting*")
            .And.ParamName.Should().Be("country");
    }
}
