using Snebur.Core.Helpers;

namespace Snebur.Core.UnitTests.Helpers;

public class PasswordHelperTests
{
    [Theory]
    [InlineData("", PasswordStrength.Empty)]
    [InlineData("short", PasswordStrength.Weak)]
    [InlineData("longerPassword", PasswordStrength.Medium)]
    [InlineData("LongerPassword1!", PasswordStrength.Strong)]
    public void CalculateStrength_ShouldReturnExpectedStrength(string password, PasswordStrength expectedStrength)
    {
        var result = PasswordHelper.CalculateStrength(password);
        result.Should().Be(expectedStrength);
    }

    [Fact]
    public void HashPassword_ShouldReturnExpectedHash()
    {
        var password = "TestPassword";
        var salt = "TestSalt";
        var expectedHash = HashHelper.CreateSha256Hash($"{password}::{salt}");

        var result = PasswordHelper.HashPassword(password, salt);

        result.Should().Be(expectedHash);
    }

    [Fact]
    public void VerifyPassword_ShouldReturnTrueForValidPassword()
    {
        var password = "TestPassword";
        var salt = "TestSalt";
        var passwordHash = PasswordHelper.HashPassword(password, salt);

        var result = PasswordHelper.VerifyPassword(password, passwordHash, salt);

        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_ShouldReturnFalseForInvalidPassword()
    {
        var password = "TestPassword";
        var salt = "TestSalt";
        var passwordHash = PasswordHelper.HashPassword(password, salt);

        var result = PasswordHelper.VerifyPassword("WrongPassword", passwordHash, salt);

        result.Should().BeFalse();
    }

    [Fact]
    public void GenerateRandomPassword_ShouldReturnPasswordOfSpecifiedLength()
    {
        var length = 16;
        var result = PasswordHelper.GenerateRandomPassword(length);

        result.Length.Should().Be(length);
    }

    [Fact]
    public void GenerateRandomPassword_ShouldThrowExceptionForInvalidLength()
    {
        Action act = () => PasswordHelper.GenerateRandomPassword(3);

        act.Should().Throw<ArgumentException>() 
            .WithMessage("Password length must be at least 4");
    }
}
