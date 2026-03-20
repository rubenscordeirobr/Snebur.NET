namespace Snebur.Core.UnitTests.Utils;

public static class RandomUtilsTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public static void GenerateRandomNumber_Should_ReturnStringOfSpecifiedLength(int length)
    {
        // Act
        var result = RandomUtils.GenerateRandomNumber(length);

        // Assert
        result.Should().HaveLength(length);
    }

    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(15)]
    public static void GenerateRandomNumber_Should_ContainOnlyDigits(int length)
    {
        // Act
        var result = RandomUtils.GenerateRandomNumber(length);

        // Assert
        foreach (var c in result)
        {
            char.IsDigit(c).Should().BeTrue();
        }
    }
}

