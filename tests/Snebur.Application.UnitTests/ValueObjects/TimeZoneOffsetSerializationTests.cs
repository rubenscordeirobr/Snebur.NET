namespace Snebur.Application.UnitTests.ValueObjects;

public class TimeZoneOffsetSerializationTests
{
    [Fact]
    public void ToJson_ShouldSerializeTimeZoneOffsetCorrectly()
    {
        // Arrange
        var result = TimeZoneOffset.Create("05:00", "SomeLocation");
        result.IsSuccess.Should().BeTrue();
        var timeZoneOffset = result.Value;

        // Act
        var json = JsonUtils.Serialize(timeZoneOffset);

        // Assert
        json.Should()
            .MatchRegex(@$"""offset""\s*:\s*""05:00""");

        json.Should()
            .MatchRegex(@$"""location""\s*:\s*""SomeLocation""");
    }

    [Fact]
    public void Parse_ShouldDeserializeTimeZoneOffsetCorrectly()
    {
        // Arrange
        var json = "{\"Offset\":\"05:00\",\"Location\":\"SomeLocation\"}";

        // Act
        var timeZoneOffset = JsonUtils.Deserialize<TimeZoneOffset>(json);

        // Assert
        timeZoneOffset.Should().NotBeNull();
        timeZoneOffset!.Offset.Should().Be("05:00");
        timeZoneOffset.Location.Should().Be("SomeLocation");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Parse_ShouldReturnDefault_ForNullOrWhitespaceInput(string? input)
    {
        // Act
        var timeZoneOffset = TimeZoneOffset.Parse(input!);

        // Assert
        timeZoneOffset.Offset.Should().Be(TimeZoneOffset.Default.Offset);
        timeZoneOffset.Location.Should().Be(TimeZoneOffset.Default.Location);
    }
}
