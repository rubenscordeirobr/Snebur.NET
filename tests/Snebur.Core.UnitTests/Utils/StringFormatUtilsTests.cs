namespace Snebur.Core.UnitTests.Utils;

public class StringFormatUtilsTests
{
    [Fact]
    public void Format_NoArgs_ShouldReturnSameString()
    {
        // Arrange
        var input = "Hello world";

        // Act
        var result = StringFormatUtils.Format(input);

        // Assert
        result.Should().Be(input);
    }

    [Fact]
    public void Format_WithSingleArg_ShouldReplacePlaceholder()
    {
        // Arrange
        var input = "Hello {name}";
        // Here, the regex replaces the placeholder using the argument at index = Math.Min(args.Length-1, match.Index).
        // For this input, match.Index will be >= 0 so it will always select args[0].
        var expected = "Hello World";

        // Act
        var result = StringFormatUtils.Format(input, "World");

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Format_WithMultiplePlaceholders_ShouldReplaceAllWithSameArgWhenIndexExceeds()
    {
        // Arrange
        var input = "Value: {a} and {b}";
        // With two arguments, the replacement for both placeholders will use the argument at index:
        // Math.Min(1, match.Index). Given match.Index of both placeholders is >= 1, both will be replaced by args[1] ("Second")
        var expected = "Value: Second and Second";

        // Act
        var result = StringFormatUtils.Format(input, "First", "Second");

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Format_NullFormat_ShouldThrowArgumentNullException()
    {
        // Arrange
        Action act = () => StringFormatUtils.Format(null!, "test");

        // Act & Assert
        act.Should().Throw<ArgumentNullException>();
    }
}

