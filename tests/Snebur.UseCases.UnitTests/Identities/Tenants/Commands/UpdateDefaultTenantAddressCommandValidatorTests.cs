using Snebur.UseCases.Identities.Tenants.Commands;

namespace Snebur.UseCases.UnitTests.Identities.Tenants.Commands;

public class UpdateDefaultTenantAddressCommandValidatorTests : IClassFixture<ServiceProviderMock<AnonymousRole>>
{
    private readonly IValidator<UpdateDefaultTenantAddressCommand> _validator;
    private readonly UpdateDefaultTenantAddressCommand _validadeCommand;

    public UpdateDefaultTenantAddressCommandValidatorTests(
        ServiceProviderMock<AnonymousRole> serviceProviderMock,
        ITestOutputHelper testOutput)
    {
        serviceProviderMock.AddTestOutput(testOutput);

        _validator = serviceProviderMock.GetRequiredService<IValidator<UpdateDefaultTenantAddressCommand>>();
        _validadeCommand = new UpdateDefaultTenantAddressCommand
        {
            Tenant_Id = Guid.NewGuid(),
            AddressName = "Address name",
            Address = new AddressDto
            {
                AddressName = "Valid Address Name",
                Street = "Street",
                Number = "123",
                Complement = "Complement",
                Neighborhood = "Neighborhood",
                City = "City",
                State = "PR",
                ZipCode = "8570555",
                Country = Country.Brazil
            }
        };
    }

    [Fact]
    public void Validator_ShouldBe_UpdateDefaultTenantAddressCommandValidator()
    {
        _validator.Should()
            .BeOfType<UpdateDefaultTenantAddressCommandValidator>();
    }

    [Fact]
    public async Task ValidationResult_ShouldBeValid()
    {
        // Act
        var result = await _validator.TestValidateAsync(_validadeCommand, cancellationToken: TestContext.Current.CancellationToken);
        
        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
        [Fact]
    public async Task ValidationResult_ShouldHaveError_When_Tenant_Id_IsInvalid()
    {
        // Arrange
        var command = _validadeCommand with
        {
            Tenant_Id = Guid.Empty
        };
         
        // Act
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Tenant_Id);
    }

    [Fact]
    public async Task ValidationResult_ShouldHaveError_When_AddressName_IsInvalid()
    {
        // Arrange
        var command = _validadeCommand with
        {
            AddressName = string.Empty
        };

        // Act
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AddressName);
    }

    [Fact]
    public async Task ValidationResult_ShouldHaveError_When_AddressName_MaxLength_IsExceeded()
    {
        // Arrange
        // Assuming ValidationConstants.AddressNameMaxLength is defined elsewhere.
        var tooLongName = new string('A', ValidationConstants.AddressNameMaxLength + 1);
        var command = _validadeCommand with
        {
            AddressName = tooLongName
        };
         
        // Act
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AddressName);
    }

    [Fact]
    public async Task ValidationResult_ShouldHaveError_When_Address_IsNull()
    {
        // Arrange
        var command = _validadeCommand with
        {
            Address = null!
        };

        // Act
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Address);
    }

    [Fact]
    public async Task ValidationResult_ShouldHaveError_When_Address_IsInvalid()
    {
        // Arrange
        var command = _validadeCommand with
        {
            Address = new AddressDto
            {
                AddressName = "Valid Address Name",
                Street = string.Empty,
                Number = string.Empty,
                Complement = null,
                Neighborhood = string.Empty,
                City = string.Empty,
                State = string.Empty,
                ZipCode = string.Empty,
                Country = Country.Unknown
            }!
        };

        // Act
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldHaveValidationErrors();
    }
}

