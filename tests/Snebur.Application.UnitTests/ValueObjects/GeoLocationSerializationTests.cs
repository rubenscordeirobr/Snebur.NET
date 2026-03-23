namespace Snebur.Application.UnitTests.ValueObjects;

public class GeoLocationSerializationTests
{
    [Fact]
    public void ToJson_ShouldSerializeGeoLocationCorrectly()
    {
        // Arrange
        var geoLocation = new GeoLocation { Latitude = 45.0, Longitude = 90.0 };

        // Act
        var json = JsonUtils.Serialize(geoLocation);

        // Assert using regex to allow for variable spaces
        json.Should().MatchRegex(@"""latitude""\s*:\s*45(\.0+)?");
        json.Should().MatchRegex(@"""longitude""\s*:\s*90(\.0+)?");
    }

    [Fact]
    public void Parse_ShouldDeserializeGeoLocationCorrectly()
    {
        // Arrange
        var json = "{\"Latitude\":45.0,\"Longitude\":90.0}";

        // Act
        var geoLocation = JsonUtils.Deserialize<GeoLocation>(json);

        // Assert
        geoLocation!.Latitude.Should().Be(45.0);
        geoLocation.Longitude.Should().Be(90.0);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Parse_ShouldReturnEmptyGeoLocation_ForNullOrWhitespace(string? input)
    {
        // Act
        var geoLocation = GeoLocation.Parse(input!);

        // Assert
        geoLocation.Should().Be(GeoLocation.Empty);
    }
}
