namespace Snebur.Core.UnitTests.Utils;

public class OperationParameterUtilsTests
{
    [Theory]
    [InlineData("ABCdef", "abcdef")]
    [InlineData("ABCdEf", "abcdEf")]
    [InlineData("Abcdef", "abcdef")]
    [InlineData("abcdef", "abcdef")]
    public void NormalizeKey_Should_Transform_Correctly(string input, string expected)
    {
        // Act
        var result = OperationParameterUtils.NormalizeKey(input);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void NormalizeKey_Should_ThrowException_When_Input_Is_NullOrWhiteSpace(string? input)
    {
        // Act
        Action act = () => OperationParameterUtils.NormalizeKey(input);

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}

