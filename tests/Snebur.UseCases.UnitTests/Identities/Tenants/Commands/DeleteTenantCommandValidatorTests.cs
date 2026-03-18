using Snebur.UseCases.Identities.Tenants.Commands;

namespace Snebur.UseCases.UnitTests.Identities.Tenants.Commands;
public class DeleteTenantCommandValidatorTests : IClassFixture<ServiceProviderMock<AnonymousRole>>
{
    private readonly IValidator<DeleteTenantCommand> _validator;

    public DeleteTenantCommandValidatorTests(ServiceProviderMock<AnonymousRole> serviceProviderMock,
        ITestOutputHelper testOutput)
    {
        serviceProviderMock.AddTestOutput(testOutput);

        _validator = serviceProviderMock.GetRequiredService<IValidator<DeleteTenantCommand>>();
    }

    [Fact]
    public void Validator_ShouldBe_DeleteTenantCommandValidator()
    {
        _validator.Should().BeOfType<DeleteTenantCommandValidator>();
    }

    [Fact]
    public void Should_Have_Error_When_Id_Is_Empty()
    {
        // Arrange
        var command = new DeleteTenantCommand(Guid.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Tenant_Id)
            .WithErrorMessage("Id is required.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Id_Is_Valid()
    {
        // Arrange
        var command = new DeleteTenantCommand(Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.Tenant_Id);
    }
}
