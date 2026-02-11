namespace Snebur.Core.UnitTests.Utils;

public class ValidationUtilsTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("1234567890")]
    [InlineData("abcdef1234567890abcdef1234567890abcdef1234567890abcdef1234567890g")]
    public void IsSha256_ShouldReturnFalse_ForInvalidInputs(string? input)
    {
        // Act
        var result = ValidationUtils.IsSha256(input);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("abcdef1234567890abcdef1234567890abcdef1234567890abcdef1234567890")]
    [InlineData("ABCDEF1234567890ABCDEF1234567890ABCDEF1234567890ABCDEF1234567890")]
    public void IsSha256_ShouldReturnTrue_ForValidSha256(string input)
    {
        // Act
        var result = ValidationUtils.IsSha256(input);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("1234567890")]
    [InlineData("+1234567890")]
    [InlineData("123-456-7890")]
    public void IsPhoneNumberValid_ShouldReturnFalse_ForInvalidPhoneNumbers(
        string? phoneNumber)
    {
        // Act
        var result = ValidationUtils.IsFullPhoneNumberValid(phoneNumber);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("+1 456 789 0123")]
    [InlineData("+55 42 3623 1870")]
    [InlineData("+55 42 93623 1870")]
    [InlineData("+441234567890")]
    public void IsPhoneNumberValid_ShouldReturnTrue_ForValidPhoneNumbers(string phoneNumber)
    {
        // Act
        var result = ValidationUtils.IsFullPhoneNumberValid(phoneNumber);

        // Assert
        result.Should().BeTrue();
    }
}
