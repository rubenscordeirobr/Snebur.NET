using Snebur.SharedKernel.Abstractions;
using Snebur.UseCases.Abstractions.Identities;
using Snebur.UseCases.Identities.Users.TenantUsers.Commands;
using Moq;

namespace Snebur.UseCases.UnitTests.Identities.Users.TenantUsers.Commands;

public class CreateTenantUserCommandValidatorTests : IClassFixture<ServiceProviderMock<AnonymousRole>>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IValidator<CreateTenantUserCommand> _validator;
    private readonly CreateTenantUserCommand _validCommand;
    public CreateTenantUserCommandValidatorTests(ServiceProviderMock<AnonymousRole> serviceProviderMock,
        ITestOutputHelper testOutput)
    {
        serviceProviderMock.AddTestOutput(testOutput);

        _serviceProvider = serviceProviderMock;
        _validator = serviceProviderMock.GetRequiredService<IValidator<CreateTenantUserCommand>>();

        _validCommand = new CreateTenantUserCommand
        {
            Tenant_Id = Guid.NewGuid(),
            Name = "Valid Tenant User",
            Email = "tenantuser@example.com",
            Password = "Password123!",
            PhoneNumber = new PhoneNumber("+5542999990000"),
            Role = UserRole.Admin
        };
    }

    [Fact]
    public void Validator_ShouldBe_CreateTenantUserCommandValidator()
    {
        _validator.Should().BeOfType<CreateTenantUserCommandValidator>();
    }

    [Fact]
    public async Task ValidationResult_ShouldBeValid()
    {
        // Act
        var result = await _validator.TestValidateAsync(_validCommand, cancellationToken: TestContext.Current.CancellationToken);
        // Assert
        result.Errors.Should().BeEmpty("All properties are valid and unique validations pass");
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task ValidationResult_ShouldHaveError_When_TenantId_IsEmpty()
    {
        // Arrange
        var command = _validCommand with { Tenant_Id = Guid.Empty };
        // Act
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Tenant_Id);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("A")]
    public async Task ValidationResult_ShouldHaveError_When_Name_IsInvalid(string? name)
    {
        // Arrange
        var command = _validCommand with { Name = name! };
        // Act
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("invalidemail")]
    [InlineData("invalid@")]
    public async Task ValidationResult_ShouldHaveError_When_Email_IsInvalid(string? email)
    {
        // Arrange
        var command = _validCommand with { Email = email! };
        // Act
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task ValidationResult_ShouldHaveError_When_Password_IsInvalid(string? password)
    {
        // Arrange
        var command = _validCommand with { Password = password! };
        // Act
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("123")]
    public async Task ValidationResult_ShouldHaveError_When_PhoneNumber_IsInvalid(string? phoneNumber)
    {
        // Arrange
        var command = _validCommand with { PhoneNumber = new PhoneNumber(phoneNumber!) };
        // Act
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Theory]
    [InlineData((UserRole)(-1))]
    public async Task ValidationResult_ShouldHaveError_When_Role_IsInvalid(UserRole role)
    {
        // Arrange
        var command = _validCommand with { Role = role };
        // Act
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Role);
    }

    [Fact]
    public async Task ValidationResult_ShouldEmailBeUnique()
    {
        // Arrange
        var localizer = _serviceProvider.GetRequiredService<IJsonStringLocalizer<CreateTenantUserCommand>>();
        var tenantValidationSetup = new Mock<ITenantUserValidationService>();

        tenantValidationSetup
            .Setup(x => x.IsEmailUniqueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        tenantValidationSetup.Setup(x => x.IsPhoneNumberUniqueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var validator = new CreateTenantUserCommandValidator(tenantValidationSetup.Object, localizer);

        // Act
        var result = await validator.TestValidateAsync(_validCommand, cancellationToken: TestContext.Current.CancellationToken);
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public async Task ValidationResult_ShouldPhoneNumberBeUnique()
    {
        // Arrange
        var localizer = _serviceProvider.GetRequiredService<IJsonStringLocalizer<CreateTenantUserCommand>>();
        var tenantValidationSetup = new Mock<ITenantUserValidationService>();

        tenantValidationSetup
            .Setup(x => x.IsEmailUniqueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        tenantValidationSetup.Setup(x => x.IsPhoneNumberUniqueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var validator = new CreateTenantUserCommandValidator(tenantValidationSetup.Object, localizer);

        // Act
        var result = await validator.TestValidateAsync(_validCommand, cancellationToken: TestContext.Current.CancellationToken);
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
    }
}

