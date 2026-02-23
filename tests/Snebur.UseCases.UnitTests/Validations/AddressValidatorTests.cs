namespace Snebur.UseCases.UnitTests.Validations;

public class AddressValidatorTests : IClassFixture<ServiceProviderMock<AnonymousRole>>
{
    private readonly IValidator<AddressDto> _validator;
    private readonly AddressDto _validAddress;

    public AddressValidatorTests(
        ServiceProviderMock<AnonymousRole> serviceProviderMock,
        ITestOutputHelper testOutput)
    {
        serviceProviderMock.AddTestOutput(testOutput);

        _validator = serviceProviderMock.GetRequiredService<IValidator<AddressDto>>();

        _validAddress = new AddressDto
        {
            AddressName = "Valid Address Name",
            Neighborhood = "Valid Neighborhood",
            Street = "Valid Street",
            Number = "123",
            ZipCode = "12345-678",
            City = "Valid City",
            State = "PR",
            Country = Country.Brazil,
            Complement = null
        };
    }

    [Fact]
    public void Validator_ShouldBe_AddressValidator()
    {
        _validator.Should().BeOfType<AddressValidator>();
    }

    [Fact]
    public void ValidationResult_ShouldBeValid()
    {
        // Arrange
        var address = _validAddress;

        // Act
        var result = _validator.TestValidate(address);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("ThisStreetNameIsWayTooLongForTheValidationToPass" + ContantesTest.StringToLongMoreThan255)]
    public void ValidationResult_ShouldHaveError_When_Street_IsInvalid(string? street)
    {
        // Arrange
        var address = _validAddress with
        {
            Street = street!
        };

        // Act
        var result = _validator.TestValidate(address);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Street);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("ThisNumberIsWayTooLongForTheValidationToPass")]
    public void ValidationResult_ShouldHaveError_When_Number_IsInvalid(string? number)
    {
        // Arrange
        var address = _validAddress with { Number = number! };

        // Act
        var result = _validator.TestValidate(address);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Number);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("ThisZipCodeIsWayTooLongForTheValidationToPass")]
    public void ValidationResult_ShouldHaveError_When_ZipCode_IsInvalid(string? zipCode)
    {
        // Arrange
        var address = _validAddress with { ZipCode = zipCode! };

        // Act
        var result = _validator.TestValidate(address);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ZipCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("ThisCityNameIsWayTooLongForTheValidationToPass" + ContantesTest.StringToLongMoreThan100)]
    public void ValidationResult_ShouldHaveError_When_City_IsInvalid(string? city)
    {
        // Arrange
        var address = _validAddress with
        {
            City = city!
        };

        // Act
        var result = _validator.TestValidate(address);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.City);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("ThisStateNameIsWayTooLongForTheValidationToPass")]
    public void ValidationResult_ShouldHaveError_When_State_IsInvalid(string? state)
    {
        // Arrange
        var address = _validAddress with
        {
            State = state!
        };

        // Act
        var result = _validator.TestValidate(address);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.State);
    }

    [Theory]
    [InlineData(null)]
    [InlineData((Country)(-1))]
    [InlineData(Country.Unknown)]
    public void ValidationResult_ShouldHaveError_When_Country_IsInvalid(Country? country)
    {
        // Arrange
        var address = _validAddress with
        {
            Country = country.GetValueOrDefault()
        };

        // Act
        var result = _validator.TestValidate(address);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Country);
    }
}
