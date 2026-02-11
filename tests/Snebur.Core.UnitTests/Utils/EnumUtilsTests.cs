namespace Snebur.Core.UnitTests.Utils;

public class EnumUtilsTests
{
    public enum TestEnum
    {
        ValueOne,
        ValueTwo,
        ValueThree
    }

    [Theory]
    [InlineData("ValueOne", TestEnum.ValueOne)]
    [InlineData("valueone", TestEnum.ValueOne)]
    [InlineData("VALUEONE", TestEnum.ValueOne)]
    [InlineData("ValueTwo", TestEnum.ValueTwo)]
    [InlineData("ValueThree", TestEnum.ValueThree)]
    public void TryParse_ValidValues_ShouldReturnTrue(
        string input,
        TestEnum expected)
    {
        var result = EnumUtils.TryParse(input, out TestEnum actual);
        result.Should().BeTrue();
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("InvalidValue")]
    public void TryParse_InvalidValues_ShouldReturnFalse(
        string? input)
    {
        var result = EnumUtils.TryParse(input, out TestEnum actual);
        result.Should().BeFalse();
        actual.Should().Be(default);
    }

    [Theory]
    [InlineData("ValueOne", TestEnum.ValueOne)]
    [InlineData("valueone", TestEnum.ValueOne)]
    [InlineData("VALUEONE", TestEnum.ValueOne)]
    [InlineData("ValueTwo", TestEnum.ValueTwo)]
    [InlineData("ValueThree", TestEnum.ValueThree)]
    public void Parse_ValidValues_ShouldReturnExpectedEnum(
        string input,
        TestEnum expected)
    {
        var result = EnumUtils.Parse<TestEnum>(input);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("InvalidValue")]
    public void Parse_InvalidValues_ShouldThrowArgumentException(
        string? input)
    {
        Action act = () => EnumUtils.Parse<TestEnum>(input);
        act.Should().Throw<ArgumentException>();
    }
}
