using Snebur.UseCases.Identities.Users.TenantUsers.Commands;

namespace Snebur.UseCases.UnitTests.Identities.Users.TenantUsers.Commands;

public class DeleteTenantUserCommandValidatorTests : IClassFixture<ServiceProviderMock<AnonymousRole>>
{
    private readonly IValidator<DeleteTenantUserCommand> _validator;

    public DeleteTenantUserCommandValidatorTests(ServiceProviderMock<AnonymousRole> serviceProviderMock,
        ITestOutputHelper testOutput)
    {
        serviceProviderMock.AddTestOutput(testOutput);

        _validator = serviceProviderMock.GetRequiredService<IValidator<DeleteTenantUserCommand>>();
    }

    [Fact]
    public void Validator_ShouldBe_DeleteTenantCommandValidator()
    {
        _validator.Should().BeOfType<DeleteTenantUserCommandValidator>();
    }

    [Fact]
    public void Should_Have_Error_When_Id_Is_Empty()
    {
        // Arrange
        var command = new DeleteTenantUserCommand(Guid.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Id)
            .WithErrorMessage("Id is required.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Id_Is_Valid()
    {
        // Arrange
        var command = new DeleteTenantUserCommand(Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.Id);
    }
}

