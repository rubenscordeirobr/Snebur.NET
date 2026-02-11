namespace Snebur.Core.UnitTests.Extensions;

public class StringExtensionsTests
{
    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("test", false)]
    public void IsNullOrEmpty_ShouldReturnExpectedResult(string? input, bool expected)
    {
        input.IsNullOrEmpty()
            .Should()
            .Be(expected);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("   ", true)]
    [InlineData("test", false)]
    public void IsNullOrWhiteSpace_ShouldReturnExpectedResult(string? input, bool expected)
    {
        input.IsNullOrWhiteSpace()
            .Should()
            .Be(expected);
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("abc123", "123")]
    [InlineData("a1b2c3", "123")]
    public void GetOnlyNumbers_ShouldReturnExpectedResult(string? input, string expected)
    {
        input.GetOnlyNumbers()
            .Should()
            .Be(expected);
    }

    [Theory]
    [InlineData(null, "", "")]
    [InlineData("", "", "")]
    [InlineData("abc123", "b", "b123")]
    [InlineData("abc123", "abc", "abc123")]
    [InlineData("a1b2c3", "abc", "a1b2c3")]
    [InlineData("a1b2c3", "c", "12c3")]
    public void GetOnlyNumbers_WithAllowedChars_ShouldReturnExpectedResult(string? input, string allowedChars, string expected)
    {
        input.GetOnlyNumbers(allowedChars)
            .Should()
            .Be(expected);
    }

    [Theory]
    [InlineData(null, new char[] { }, "")]
    [InlineData("", new char[] { }, "")]
    [InlineData("abc123", new char[] { 'a', 'b', 'c' }, "abc123")]
    [InlineData("a1b2c3", new char[] { 'a', 'b', 'c' }, "a1b2c3")]
    public void GetOnlyNumbers_WithAllowedCharsArray_ShouldReturnExpectedResult(
        string? input, char[] allowedChars, string expected)
    {
        input.GetOnlyNumbers(allowedChars)
            .Should()
            .Be(expected);
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("abc123", "abc")]
    [InlineData("a1b2c3", "abc")]
    public void GetOnlyLetters_ShouldReturnExpectedResult(string? input, string expected)
    {
        input.GetOnlyLetters().Should().Be(expected);
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("abc123!@#", "!@#")]
    [InlineData("a1b2c3$%^", "$%^")]
    public void GetOnlySpecialChars_ShouldReturnExpectedResult(string? input, string expected)
    {
        input.GetOnlySpecialChars()
            .Should()
            .Be(expected);
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("abc123", "abc123")]
    [InlineData("a1b2c3", "a1b2c3")]
    public void GetOnlyLettersAndNumbers_ShouldReturnExpectedResult(string? input, string expected)
    {
        input.GetOnlyLettersAndNumbers()
            .Should()
            .Be(expected);
    }

    [Theory]
    [InlineData(null, 0, "")]
    [InlineData("abc", -1, "abc")]
    [InlineData("abc", 1, "bc")]
    [InlineData("abc", 5, "")]
    public void SafeSubstring_ShouldReturnExpectedResult(string? input, int startIndex, string expected)
    {
        input.SafeSubstring(startIndex)
            .Should()
            .Be(expected);
    }

    [Theory]
    [InlineData(null, 0, 0, "")]
    [InlineData("abc", -1, 2, "ab")]
    [InlineData("abc", 1, 2, "bc")]
    [InlineData("abc", 1, 5, "bc")]
    public void SafeSubstring_WithLength_ShouldReturnExpectedResult(string? input, int startIndex, int length, string expected)
    {
        input.SafeSubstring(startIndex, length)
            .Should()
            .Be(expected);
    }

    [Theory]
    [InlineData(null, 0, "")]
    [InlineData(" abc ", 3, "abc")]
    [InlineData(" abc ", 1, "a")]
    public void SafeTrim_ShouldReturnExpectedResult(string? input, int maxLength, string expected)
    {
        input.SafeTrim(maxLength)
            .Should()
            .Be(expected);
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData(" test", "Test")]
    [InlineData("Test", "Test")]
    public void Capitalize_ShouldReturnExpectedResult(string? input, string expected)
    {
        input.Capitalize()
            .Should()
            .Be(expected);
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData(" Test", "test")]
    [InlineData("Test", "test")]
    public void Uncapitalize_ShouldReturnExpectedResult(string? input, string expected)
    {
        input.Uncapitalize()
            .Should()
            .Be(expected);
    }
}
