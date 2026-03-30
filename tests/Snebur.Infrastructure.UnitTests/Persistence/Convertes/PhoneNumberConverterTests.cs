namespace Snebur.Infrastructure.UnitTests.Persistence.Convertes;

public class PhoneNumberConverterTests
{
    [Fact]
    public void ConvertToProvider_Should_Return_StringValue()
    {
        // Arrange
        var phoneNumberValue = "+5511987651234";
        var phoneNumber = new PhoneNumber(phoneNumberValue);
        var converter = PhoneNumberConverter.Instance;
        var convertToProvider = converter.ConvertToProviderExpression.Compile(); 

        // Act
        var result = convertToProvider(phoneNumber);

        // Assert
        result.Should().Be(phoneNumberValue);
    }

    [Fact]
    public void ConvertFromProvider_Should_Return_PhoneNumber_With_Correct_Value()
    {
        // Arrange
        var phoneNumberValue = "+5511987651234";
        var converter = PhoneNumberConverter.Instance;
        var convertFromProvider = converter.ConvertFromProviderExpression.Compile();

        // Act
        var result = convertFromProvider(phoneNumberValue);

        // Assert
        result.Should().NotBeNull();
        result.FullNumber.Should().Be(phoneNumberValue);
    }
}
