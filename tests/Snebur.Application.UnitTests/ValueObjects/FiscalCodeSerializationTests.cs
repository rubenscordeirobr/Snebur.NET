namespace Snebur.Application.UnitTests.ValueObjects;

public class FiscalCodeSerializationTests
{
    [Fact]
    public void SerializeFiscalCode_ShouldProduceJsonWithProcessedValue()
    {
        // Arrange
        var originalValue = "ABC123456789XYZ"; // Non-numeric characters included.
        // Assuming GetOnlyNumbers strips letters and leaves only digits.
        var expectedValue = "123456789";
        var fiscalCode = new FiscalCode(originalValue);

        // Act
        var json = JsonUtils.Serialize(fiscalCode);

        // Assert
        json.Should()
            .MatchRegex(@$"""value""\s*:\s*""{expectedValue}""");
    }

    [Fact]
    public void DeserializeFiscalCode_ShouldProduceFiscalCodeWithProcessedValue()
    {
        // Arrange
        var json = "{\"Value\":\"ABC123456789XYZ\"}";
        var expectedValue = "123456789";

        // Act
        var fiscalCode = JsonUtils.Deserialize<FiscalCode>(json);

        // Assert
        fiscalCode.Should().NotBeNull();
        fiscalCode!.Value.Should().Be(expectedValue);
    }
}
