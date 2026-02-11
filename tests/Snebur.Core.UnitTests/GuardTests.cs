namespace Snebur.Core.UnitTests;

public class GuardTests
{
    [Theory]
    [InlineData(null)]
    public void NotNull_ShouldThrowArgumentNullException_WhenValueIsNull(object? value)
    {
        Action act = () => Guard.NotNull(value);

        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void NotNullOrWhiteSpace_ShouldThrowArgumentException_WhenValueIsNullOrWhiteSpace(string? value)
    {
        Action act = () => Guard.NotNullOrWhiteSpace(value);

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("123")]
    [InlineData("invalid-phone-number")]
    public void FullPhoneNumber_ShouldThrowArgumentException_WhenValueIsInvalid(string? value)
    {
        Action act = () => Guard.FullPhoneNumber(value);

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Positive_ShouldThrowArgumentOutOfRangeException_WhenValueIsNotPositive(int value)
    {
        Action act = () => Guard.Positive(value);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("invalid-sha256")]
    public void Sha256_ShouldThrowArgumentException_WhenValueIsNotSha256(string? value)
    {
        Action act = () => Guard.Sha256(value);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void NotEmpty_ShouldThrowArgumentException_WhenValueIsEmptyInt()
    {
        int value = default;

        Action act = () => Guard.NotEmpty(value, nameof(value));

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    public void NotEmpty_ShouldNotThrow_WhenValueIsNotEmptyInt(int value)
    {
        Action act = () => Guard.NotEmpty(value, nameof(value));

        act.Should().NotThrow();
    }

    [Fact]
    public void NotEmpty_ShouldThrowArgumentException_WhenValueIsEmptyGuid()
    {
        Guid value = default;

        Action act = () => Guard.NotEmpty(value, nameof(value));

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void NotEmpty_ShouldNotThrow_WhenValueIsNotEmptyGuid()
    {
        Guid value = Guid.NewGuid();
        Action act = () => Guard.NotEmpty(value);

        act.Should().NotThrow();
    }

    [Fact]
    public void MustBeEmpty_ShouldNotThrow_WhenValueIsEmptyInt()
    {
        int value = default;

        Action act = () => Guard.MustBeEmpty(value);

        act.Should().NotThrow();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    public void MustBeEmpty_ShouldThrowArgumentException_WhenValueIsNotEmptyInt(int value)
    {
        Action act = () => Guard.MustBeEmpty(value);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void MustBeEmpty_ShouldNotThrow_WhenValueIsEmptyGuid( )
    {
        Guid value = Guid.Empty;
         
        Action act = () => Guard.MustBeEmpty(value);

        act.Should().NotThrow();
    }

    [Fact]
    public void MustBeEmpty_ShouldThrowArgumentException_WhenValueIsNotEmptyGuid()
    {
        Guid value = Guid.NewGuid();
        Action act = () => Guard.MustBeEmpty(value);

        act.Should().Throw<ArgumentException>();
    }
}
