namespace Snebur.Infrastructure.UnitTests.Persistence.Convertes;

public class FiscalCodeConverterTests
{
    [Fact]
    public void ConvertToProvider_Should_Return_StringValue()
    {
        // Arrange
        var fiscalCodeValue = "123456789";
        var fiscalCode = new FiscalCode(fiscalCodeValue);
        var converter = FiscalCodeConverter.Instance;
        var convertToProvider = converter.ConvertToProviderExpression.Compile();

        // Act
        var result = convertToProvider(fiscalCode);

        // Assert
        result.Should().Be(fiscalCodeValue);
    }

    [Fact]
    public void ConvertFromProvider_Should_Return_FiscalCode_With_Correct_Value()
    {
        // Arrange
        var fiscalCodeValue = "987654321";
        var converter = FiscalCodeConverter.Instance;
        var convertFromProvider = converter.ConvertFromProviderExpression.Compile();

        // Act
        var result = convertFromProvider(fiscalCodeValue);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be(fiscalCodeValue);
    }
}
