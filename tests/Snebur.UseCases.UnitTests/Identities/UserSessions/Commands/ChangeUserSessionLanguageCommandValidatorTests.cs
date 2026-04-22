using Snebur.UseCases.Identities.UserSessions;

namespace Snebur.UseCases.UnitTests.Identities.UserSessions.Commands;

public class ChangeUserSessionLanguageCommandValidatorTests : IClassFixture<ServiceProviderMock<AnonymousRole>>
{
    private readonly IValidator<ChangeUserSessionLanguageCommand> _validator;

    public ChangeUserSessionLanguageCommandValidatorTests(
        ServiceProviderMock<AnonymousRole> serviceProviderMock,
        ITestOutputHelper testOutput)
    {
        serviceProviderMock.AddTestOutput(testOutput);

        _validator = serviceProviderMock.GetRequiredService<IValidator<ChangeUserSessionLanguageCommand>>();
    }

    [Fact]
    public void Validator_ShouldBe_ChangeUserSessionLanguageCommandValidator()
    {
        _validator.Should().BeOfType<ChangeUserSessionLanguageCommandValidator>();
    }

    [Fact]
    public void Should_Have_Error_When_Language_Is_Invalid()
    {
        // Arrange
        var command = new ChangeUserSessionLanguageCommand(
            Guid.NewGuid(),
            (Language)999);
        // Act
        var result = _validator.TestValidate(command);
        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Language)
            .WithErrorMessage("Invalid language.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Language_Is_Valid()
    {
        // Arrange
        var command = new ChangeUserSessionLanguageCommand(
            Guid.NewGuid(),
            Language.Default);
        // Act
        var result = _validator.TestValidate(command);
        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.Language);
    }
}
