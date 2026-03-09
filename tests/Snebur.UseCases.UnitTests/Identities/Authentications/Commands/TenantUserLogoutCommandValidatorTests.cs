using Snebur.UseCases.Identities.Authentications.Commands;

namespace Snebur.UseCases.UnitTests.Identities.Authentications.Commands;

public class TenantUserLogoutCommandValidatorTests : IClassFixture<ServiceProviderMock<AnonymousRole>>
{
    private readonly IValidator<TenantUserLogoutCommand> _validator;
    private readonly TenantUserLogoutCommand _validCommand;

    public TenantUserLogoutCommandValidatorTests(ServiceProviderMock<AnonymousRole> serviceProviderMock,
        ITestOutputHelper testOutput)
    {
        serviceProviderMock.AddTestOutput(testOutput);

        _validator = serviceProviderMock.GetRequiredService<IValidator<TenantUserLogoutCommand>>();
        _validCommand = new TenantUserLogoutCommand(Guid.NewGuid());
    }

    [Fact]
    public void Validator_ShouldBe_TenantUserLogoutCommandValidator()
    {
        _validator.Should().BeOfType<TenantUserLogoutCommandValidator>();
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

