using Snebur.SharedKernel.Abstractions;
using Snebur.UseCases.Abstractions.Identities;
using Snebur.UseCases.Identities.Authentications.Commands;
using Moq;

namespace Snebur.UseCases.UnitTests.Identities.Authentications.Commands;

public class TenantUserLoginCommandValidatorTests : IClassFixture<ServiceProviderMock<AnonymousRole>>
{
    private readonly IValidator<TenantUserLoginCommand> _validator;
    private readonly IServiceProvider _serviceProvider;
    private readonly TenantUserLoginCommand _validCommand;

    public TenantUserLoginCommandValidatorTests(
        ServiceProviderMock<AnonymousRole> serviceProviderMock,
        ITestOutputHelper testOutput)
    {
        serviceProviderMock.AddTestOutput(testOutput);

        _serviceProvider = serviceProviderMock;
        _validator = serviceProviderMock.GetRequiredService<IValidator<TenantUserLoginCommand>>();
        _validCommand = new TenantUserLoginCommand
        {
            EmailOrPhoneNumber = SystemTenantConstants.Email,
            Password = "TenantAdmin@Teste%#",
            IsPersistent = true
        };
    }

    [Fact]
    public void Validator_ShouldBe_TenantUserLoginCommandValidator()
    {
        _validator.Should().BeOfType<TenantUserLoginCommandValidator>();
    }

    [Fact]
    public async Task ValidationResult_ShouldBeValid()
    {
        // Act
        var result = await _validator.TestValidateAsync(_validCommand, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.Errors.Should().BeEmpty($"Command is valid but failed with errors: {string.Join(",", result.Errors.Select(x => x.ErrorMessage))}");
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("invalid")]
    public async Task ValidationResult_ShouldHaveError_When_EmailOrPhoneNumber_IsInvalid(
        string? emailOrPhoneNumber)
    {
        // Arrange
        var command = _validCommand with
        {
            EmailOrPhoneNumber = emailOrPhoneNumber!
        };
        // Act
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EmailOrPhoneNumber);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("short")]
    public async Task ValidationResult_ShouldHaveError_When_Password_IsInvalid(string? password)
    {
        // Arrange
        var command = _validCommand with
        {
            Password = password!
        };

        // Act
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public async Task ValidationResult_ShouldHaveError_When_EmailOrPhoneNumberNotExists()
    {
        // Arrange
        var localizer = _serviceProvider.GetRequiredService<IJsonStringLocalizer<TenantUserLoginCommand>>();
        var tenantValidationSetup = new Mock<ITenantUserAuthenticationValidationService>();

        tenantValidationSetup
            .Setup(x => x.EmailOrPhoneNumberExitsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var validator = new TenantUserLoginCommandValidator(tenantValidationSetup.Object, localizer);

        // Act
        var result = await validator.TestValidateAsync(_validCommand, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EmailOrPhoneNumber);
    }
}
