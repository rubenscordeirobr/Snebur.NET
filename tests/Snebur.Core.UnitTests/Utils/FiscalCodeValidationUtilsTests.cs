namespace Snebur.Core.UnitTests.Utils;

public class FiscalCodeValidationUtilsTests
{
    [Theory]
    [InlineData(null, Country.Brazil, false)]
    [InlineData("", Country.Brazil, false)]
    [InlineData("   ", Country.Brazil, false)]
    [InlineData("01114851000", Country.Brazil, true)] // Valid CPF
    [InlineData("011.148.510-00", Country.Brazil, true)] // Invalid CPF
    [InlineData("74.882.697/0001-08", Country.Brazil, true)] // Valid CNPJ
    [InlineData("74882697000108", Country.Brazil, true)] // Invalid CNPJ
    [InlineData("12345678909", Country.Unknown, false)]
    public void IsValid_ShouldReturnExpectedResult(string? fiscalCode, Country country, bool expectedResult)
    {
        var result = FiscalCodeValidationUtils.IsValid(fiscalCode, country);
        result.Should().Be(expectedResult);
    }
}

