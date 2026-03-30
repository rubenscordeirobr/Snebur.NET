using Snebur.UseCases.Identities.Authentications.Commands;

namespace Snebur.UseCases.UnitTests.Identities.Authentications.Commands;

public class AdminUserLogoutCommandValidatorTests : IClassFixture<ServiceProviderMock<AnonymousRole>>
{
    private readonly IValidator<AdminUserLogoutCommand> _validator;
    private readonly AdminUserLogoutCommand _validCommand;

    public AdminUserLogoutCommandValidatorTests(ServiceProviderMock<AnonymousRole> serviceProviderMock,
        ITestOutputHelper testOutput)
    {
        serviceProviderMock.AddTestOutput(testOutput);

        _validator = serviceProviderMock.GetRequiredService<IValidator<AdminUserLogoutCommand>>();
        _validCommand = new AdminUserLogoutCommand(Guid.NewGuid());
    }

    [Fact]
    public void Validator_ShouldBe_AdminUserLogoutCommandValidator()
    {
        _validator.Should().BeOfType<AdminUserLogoutCommandValidator>();
    }

    [Fact]
    public async Task ValidationResult_ShouldBeValid()
    {
        // Act
        var result = await _validator.TestValidateAsync(_validCommand, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task ValidationResult_ShouldHaveError_When_ClientSessionToken_IsEmpty()
    {
        // Arrange
        var command = _validCommand with { Session_Id = Guid.Empty };

        // Act
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Session_Id);
    }
 
}

