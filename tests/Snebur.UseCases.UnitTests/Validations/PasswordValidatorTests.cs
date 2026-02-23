using Snebur.SharedKernel.Abstractions;

namespace Snebur.UseCases.UnitTests.Validations;

public class PasswordValidatorTests : IClassFixture<ServiceProviderMock<AnonymousRole>>
{
    private readonly IValidator<Password> _validator;
    
    public PasswordValidatorTests(
        ServiceProviderMock<AnonymousRole> serviceProviderMock,
        ITestOutputHelper testOutput)
    {
        serviceProviderMock.AddTestOutput(testOutput);

        var localizer = serviceProviderMock
            .GetRequiredService<IJsonStringLocalizer<Password>>();

        _validator = new PasswordValidator(localizer);
    }

    [Fact]
    public void Validator_ShouldBePasswordValidatorType()
    {
        _validator.Should().BeOfType<PasswordValidator>();
    }

    [Fact]
    public void Validate_ShouldFail_When_PasswordStrengthIsWeak()
    {
        // Arrange
        var weakPassword = new Password(
            "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855",
            PasswordStrength.Weak);

        // Act
        var result = _validator.TestValidate(weakPassword);

        // Assert
        result.ShouldHaveValidationErrorFor(p => p.Strength);
    }

    [Fact]
    public void Validate_ShouldThrow_When_PasswordHashIsInvalid()
    {
        // Act
        Action act = () => new Password("invalid hash", PasswordStrength.Strong);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Validate_ShouldSucceed_When_PasswordIsValid()
    {
        // Arrange: valid SHA256 hash and strong strength.
        var validPassword = new Password(
            "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855",
            PasswordStrength.Strong);
        // Act
        var result = _validator.TestValidate(validPassword);

        // Assert
        result.ShouldNotHaveValidationErrorFor(p => p.HashValue);
    }
}
