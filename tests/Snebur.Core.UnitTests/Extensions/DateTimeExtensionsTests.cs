namespace Snebur.Core.UnitTests.Extensions;

public class DateTimeExtensionsTests
{
    [Theory]
    [InlineData(-1, true)]
    [InlineData(0, true)]
    [InlineData(1, false)]
    public void IsExpired_ShouldReturnExpectedResult(int secondsToAdd, bool expectedResult)
    {
        // Arrange
        var dateTime = DateTime.UtcNow.AddSeconds(secondsToAdd);
        var expirationTime = TimeSpan.FromSeconds(0);

        // Act
        var result = dateTime.IsExpired(expirationTime);

        // Assert
        result.Should().Be(expectedResult);
    }
}
