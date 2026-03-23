namespace Snebur.Core.UnitTests.Utils;

public class BrazilianFormattingUtilsTests
{
    [Theory]
    [InlineData(null, null)]
    [InlineData("", "")]
    [InlineData(" ", " ")]
    [InlineData("\t", "\t")]
    [InlineData("abc", "abc")]
    [InlineData("1234567890", "1234567890")] // Invalid length
    [InlineData("12345678901", "123.456.789-01")]
    [InlineData("123.456.789-01", "123.456.789-01")]
    [InlineData("abc12345678901xyz", "123.456.789-01")]

    public void FormatCpf_ShouldFormatCorrectly(string? input, string? expected)
    {
        var result = BrazilianFormattingUtils.FormatCpf(input!);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("", "")]
    [InlineData(" ", " ")]
    [InlineData("\t", "\t")]
    [InlineData("abc", "abc")]
    [InlineData("12345678000199", "12.345.678/0001-99")]
    [InlineData("12.345.678/0001-99", "12.345.678/0001-99")]
    [InlineData("xyz12345678000199abc", "12.345.678/0001-99")]
    [InlineData("12345678", "12345678")] // Invalid length
    public void FormatCnpj_ShouldFormatCorrectly(string? input, string? expected)
    {
        var result = BrazilianFormattingUtils.FormatCnpj(input!);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("", "")]
    [InlineData(" ", " ")]
    [InlineData("\t", "\t")]
    [InlineData("abc", "abc")]
    [InlineData("12345678", "12345-678")]
    [InlineData("12.345-678", "12345-678")]
    [InlineData("abc12345678xyz", "12345-678")]
    [InlineData("1234567", "1234567")] // Invalid length
    public void FormatCep_ShouldFormatCorrectly(string? input, string? expected)
    {
        var result = BrazilianFormattingUtils.FormatCep(input!);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(null, true, null)]
    [InlineData("", false, "")]
    [InlineData(" ", false, " ")]
    [InlineData("\t", false, "\t")]
    [InlineData("abc", false, "abc")]
    [InlineData("11987654321", false, "(11) 98765-4321")]
    [InlineData("011987654321", false, "(11) 98765-4321")]
    [InlineData("+5511987654321", false, "(11) 98765-4321")]
    [InlineData("5511987654321", true, "+55 (11) 98765-4321")]
    [InlineData("1198765432", false, "(11) 9876-5432")] // Landline format
    [InlineData("abc11987654321xyz", false, "(11) 98765-4321")]
    [InlineData("123", false, "123")] // Invalid
    public void FormatPhone_ShouldFormatCorrectly(
        string? input, 
        bool international, 
        string? expected)
    {
        var result = BrazilianFormattingUtils.FormatPhone(input!, international);
        result.Should().Be(expected);
    }

    ///\u00A0 no break space
    [Theory]
    [InlineData(123.45, "R$\u00A0123,45")]
    [InlineData(1000.99, "R$\u00A01.000,99")]
    [InlineData(0, "R$\u00A00,00")]
    [InlineData(-1, "-R$\u00A01,00")]
    public void FormatMoney_ShouldFormatAccordingToPtBr(decimal input, string expected)
    {
        var result = BrazilianFormattingUtils.FormatMoney(input);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("", "")]
    [InlineData(" ", " ")]
    [InlineData("\t", "\t")]
    [InlineData("abc", "abc")]
    [InlineData("12345678901", "123.456.789-01")]
    [InlineData("12345678000199", "12.345.678/0001-99")]
    [InlineData("abc12345678901xyz", "123.456.789-01")]
    [InlineData("xyz12345678000199abc", "12.345.678/0001-99")]
    [InlineData("000", "000")] // Not valid CPF or CNPJ
    public void FormatFiscalCode_ShouldReturnFormattedCpfOrCnpj(string? input, string? expected)
    {
        var result = BrazilianFormattingUtils.FormatFiscalCode(input!);
        result.Should().Be(expected);
    }
}
