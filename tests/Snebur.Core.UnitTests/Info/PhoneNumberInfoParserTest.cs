namespace Snebur.Core.UnitTests.Info;

public class PhoneNumberInfoParserTest
{
    
    [Theory]
    [InlineData("+5511987654321", Country.Brazil, InternationalDialingCode.Brazil, "11987654321", "11", "(11) 98765-4321")]
    [InlineData("+551197654321", Country.Brazil, InternationalDialingCode.Brazil, "1197654321", "11", "(11) 9765-4321")]
    [InlineData("+12345678901", Country.UnitedStates, InternationalDialingCode.UnitedStates, "2345678901", "234", "(234) 567-8901")]
    public void Parse_ShouldReturnCorrectPhoneNumberInfo_WhenInputIsValid(
        string fullNumber,
        Country expectedCountryCode,
        InternationalDialingCode expectedInternationalDialingCode,
        string expectedNationalNumber,
        string expectedAreaCode,
        string expectedFormattedNationalNumber)
    {
        // Act
        var result = PhoneNumberInfoParser.Parse(fullNumber);

        // Assert
        result.Country.Should().Be(expectedCountryCode);
        result.InternationalDialingCode.Should().Be(expectedInternationalDialingCode);
        result.NationalNumber.Should().Be(expectedNationalNumber);
        result.AreaCode.Should().Be(expectedAreaCode);
        result.FormattedNationalNumber.Should().Be(expectedFormattedNationalNumber);
    }

    [Theory]
    [InlineData("1234567890")]
    [InlineData("5511987654321")]
    [InlineData("441234567890")]
    [InlineData("34912345678")]
    public void Parse_ShouldReturnUnknownPhoneNumberInfo_WhenInputDoesNotStartWithPlus(string fullNumber)
    {
        // Act
        var result = PhoneNumberInfoParser.Parse(fullNumber);

        // Assert
        result.Country.Should().Be(Country.Unknown);
        result.InternationalDialingCode.Should().Be(InternationalDialingCode.Unknown);
        result.NationalNumber.Should().Be(fullNumber);
        result.AreaCode.Should().BeEmpty();
        result.FormattedNationalNumber.Should().Be(fullNumber);
    }
}
