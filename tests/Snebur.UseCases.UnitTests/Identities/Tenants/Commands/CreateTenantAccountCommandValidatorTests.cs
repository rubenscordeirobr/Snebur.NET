using Snebur.SharedKernel.Abstractions;
using Snebur.UseCases.Abstractions.Identities;
using Snebur.UseCases.Identities.Tenants.Commands;
using Moq;

namespace Snebur.UseCases.UnitTests.Identities.Tenants.Commands;

public class CreateTenantAccountCommandValidatorTests : IClassFixture<ServiceProviderMock<AnonymousRole>>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IValidator<CreateTenantAccountCommand> _validator;
    private readonly CreateTenantAccountCommand _validCommand;

    public CreateTenantAccountCommandValidatorTests(ServiceProviderMock<AnonymousRole> serviceProviderMock,
        ITestOutputHelper testOutput)
    {
        serviceProviderMock.AddTestOutput(testOutput);

        _serviceProvider = serviceProviderMock;
        _validator = serviceProviderMock.GetRequiredService<IValidator<CreateTenantAccountCommand>>();

        _validCommand = new CreateTenantAccountCommand
        {
            Name = "Tenant name",
            FiscalCode = new FiscalCode("04866748940"),
            BusinessName = "Tenant name",
            Email = "tenant1@snebur.com",
            Password = "Password123!",
            IsPersistent = false,
            Country = Country.Brazil,
            Language = Language.PortugueseBrazil,
            Currency = Currency.BRL,
            BusinessType = BusinessType.CivilRegistryOffice,
            TenantType = TenantType.Company,
            PhoneNumber = new PhoneNumber("+55 42 99977 1020")
        };
    }

    [Fact]
    public void Validator_ShouldBe_CreateTenantCommandValidator()
    {
        _validator.Should().BeOfType<CreateTenantAccountCommandValidator>();
    }

    [Fact]
    public async Task ValidationResult_ShouldBeValid()
    {
        // Arrange
        var randowEmail = Guid.NewGuid().ToString().Substring(0, 8) + "@snebur.com";
        var fakeCpf = BrazilianFakeUtils.GenerateCpf();
        var fakePhoneNumber = BrazilianFakeUtils.GenerateFakePhoneNumber();
        var command = _validCommand with
        {
            Email = randowEmail,
            FiscalCode = new FiscalCode(fakeCpf),
            PhoneNumber = new PhoneNumber(fakePhoneNumber)
        };

        // Act
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.Errors.Should()
            .BeEmpty($"Command must valid and failed with errors: {string.Join(",", result.Errors)}");

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("A")]
    [InlineData("AB")]
    [InlineData("first")]
    [InlineData("ThisNameIsWayTooLongForTheValidationToPass")]
    public async Task ValidationResult_ShouldHaveError_When_Name_IsInvalid(string? name)
    {
        // Arrange
        var command = _validCommand with
        {
            Name = name!
        };

        // Act
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("abc")]
    [InlineData(ContantesTest.StringToLongMoreThan100)]
    public async Task ValidationResult_ShouldHaveError_When_TenantName_IsInvalid(
        string? tenantName)
    {
        // Arrange
        var command = _validCommand with
        {
            BusinessName = tenantName!
        };

        // Act
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BusinessName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("invalidemail")]
    [InlineData("invalid@")]
    [InlineData("invalid@com .10")]
    [InlineData("1 invalid@com .10")]
    public async Task ValidationResult_ShouldHaveError_When_Email_IsInvalid(string? email)
    {
        // Arrange
        var command = _validCommand with
        {
            Email = email!
        };

        // Act
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("132")]
    [InlineData("13245674")]
    [InlineData("15616159616")]
    public async Task ValidationResult_ShouldHaveError_When_FiscalCode_IsInvalid(string? fiscalCode)
    {
        // Arrange
        var command = _validCommand with
        {
            FiscalCode = new FiscalCode(fiscalCode!)
        };

        // Act
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FiscalCode.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("132")]
    [InlineData("13245674")]
    public async Task ValidationResult_ShouldHaveError_When_PhoneNumber_IsInvalid(string? phoneNumber)
    {
        // Arrange
        var command = _validCommand with
        {
            PhoneNumber = new PhoneNumber(phoneNumber!)
        };

        // Act
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Theory]
    [InlineData(null)]
    [InlineData((Country)(-1))]
    [InlineData(Country.Unknown)]
    public async Task ValidationResult_ShouldHaveError_When_Country_IsInvalid(Country? country)
    {
        // Arrange
        var command = _validCommand with
        {
            Country = country.GetValueOrDefault()
        };

        // Act
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Country);
    }

    [Theory]
    [InlineData(null)]
    [InlineData((Language)(-1))]
    public async Task ValidationResult_ShouldHaveError_When_Language_IsInvalid(Language? culture)
    {
        // Arrange
        var command = _validCommand with
        {
            Language = culture.GetValueOrDefault()
        };

        // Act
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Language);

    }

    [Theory]
    [InlineData(null)]
    [InlineData((Currency)(-1))]
    public async Task ValidationResult_ShouldHaveError_When_Currency_IsInvalid(Currency? currency)
    {
        // Arrange
        var command = _validCommand with
        {
            Currency = currency.GetValueOrDefault()
        };

        // Act
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Currency);
    }

    [Theory]
    [InlineData(null)]
    [InlineData((BusinessType)(-1))]

    public async Task ValidationResult_ShouldHaveError_When_BusinessType_IsInvalid(BusinessType? businessType)
    {
        // Arrange
        var command = _validCommand with
        {
            BusinessType = businessType.GetValueOrDefault()
        };

        // Act
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BusinessType);
    }

    [Theory]
    [InlineData(null)]
    [InlineData((TenantType)(-1))]
    public async Task ValidationResult_ShouldHaveError_When_TenantType_IsInvalid(TenantType? tenantType)
    {
        // Arrange
        var command = _validCommand with
        {
            TenantType = tenantType.GetValueOrDefault()
        };
        // Act
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TenantType);
    }

    [Fact]
    public async Task ValidationResult_ShouldEmailBeUnique()
    {
        // Arrange
        var localizer = _serviceProvider.GetRequiredService<IJsonStringLocalizer<CreateTenantAccountCommand>>();

        var tenantValidationSetup = new Mock<ITenantValidationService>();

        tenantValidationSetup
            .Setup(x => x.IsEmailUniqueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        tenantValidationSetup.Setup(x => x.IsFiscalCodeUniqueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        tenantValidationSetup.Setup(x => x.IsPhoneNumberUniqueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var validator = new CreateTenantAccountCommandValidator(
            tenantValidationSetup.Object,
            localizer);

        // Act
        var result = await validator.TestValidateAsync(_validCommand, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public async Task ValidationResult_ShouldFiscalCodeBeUnique()
    {
        // Arrange
        var localizer = _serviceProvider.GetRequiredService<IJsonStringLocalizer<CreateTenantAccountCommand>>();

        var tenantValidationSetup = new Mock<ITenantValidationService>();

        tenantValidationSetup.Setup(x => x.IsFiscalCodeUniqueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        tenantValidationSetup
            .Setup(x => x.IsEmailUniqueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        tenantValidationSetup
            .Setup(x => x.IsPhoneNumberUniqueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var validator = new CreateTenantAccountCommandValidator(
            tenantValidationSetup.Object,
            localizer);

        // Act
        var result = await validator.TestValidateAsync(_validCommand, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FiscalCode);
    }

    [Fact]
    public async Task ValidationResult_ShouldPhoneNumberBeUnique()
    {
        // Arrange
        var localizer = _serviceProvider.GetRequiredService<IJsonStringLocalizer<CreateTenantAccountCommand>>();

        var tenantValidationSetup = new Mock<ITenantValidationService>();

        tenantValidationSetup
          .Setup(x => x.IsPhoneNumberUniqueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
          .ReturnsAsync(false);

        tenantValidationSetup.Setup(x => x.IsFiscalCodeUniqueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        tenantValidationSetup
            .Setup(x => x.IsEmailUniqueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var validator = new CreateTenantAccountCommandValidator(
            tenantValidationSetup.Object,
            localizer);

        // Act
        var result = await validator.TestValidateAsync(_validCommand, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
    }
}
