namespace Snebur.Infrastructure.UnitTests.Persistence.Convertes;

public class UtcDateTimeConverterTests
{
    [Fact]
    public void ConvertToProvider_With_DefaultValue_Should_Return_UtcDefault()
    {
        // Arrange
        var defaultDateTime = default(DateTime);
        var converter = UtcDateTimeConverter.Instance;
        var convertToProvider = converter.ConvertToProviderExpression.Compile();

        // Act
        var result = convertToProvider(defaultDateTime);

        // Assert
        result.Should().Be(DateTime.SpecifyKind(default, DateTimeKind.Utc));
    }

    [Fact]
    public void ConvertToProvider_With_UnspecifiedKind_Should_Convert_To_Utc()
    {
        // Arrange
        var unspecifiedDate = new DateTime(2025, 3, 22, 12, 0, 0, DateTimeKind.Unspecified);
        var converter = UtcDateTimeConverter.Instance;
        var convertToProvider = converter.ConvertToProviderExpression.Compile();

        // Act
        var result = convertToProvider(unspecifiedDate);

        // Assert
        result.Kind.Should().Be(DateTimeKind.Utc);
        result.Should().Be(unspecifiedDate);
    }

    [Fact]
    public void ConvertToProvider_With_UtcKind_Should_Return_SameValue()
    {
        // Arrange
        var utcDate = new DateTime(2025, 3, 22, 12, 0, 0, DateTimeKind.Utc);
        var converter = UtcDateTimeConverter.Instance;
        var convertToProvider = converter.ConvertToProviderExpression.Compile();

        // Act
        var result = convertToProvider(utcDate);

        // Assert
        result.Kind.Should().Be(DateTimeKind.Utc);
        result.Should().Be(utcDate);
    }

    [Fact]
    public void ConvertFromProvider_With_DefaultValue_Should_Return_UtcDefault()
    {
        // Arrange
        var defaultDateTime = default(DateTime);
        var converter = UtcDateTimeConverter.Instance;
        var convertFromProvider = converter.ConvertFromProviderExpression.Compile();

        // Act
        var result = convertFromProvider(defaultDateTime);

        // Assert
        result.Should().Be(DateTime.SpecifyKind(default, DateTimeKind.Utc));
    }

    [Fact]
    public void ConvertFromProvider_With_UnspecifiedKind_Should_Convert_To_Utc()
    {
        // Arrange
        var unspecifiedDate = new DateTime(2025, 3, 22, 12, 0, 0, DateTimeKind.Unspecified);
        var converter = UtcDateTimeConverter.Instance;
        var convertFromProvider = converter.ConvertFromProviderExpression.Compile();

        // Act
        var result = convertFromProvider(unspecifiedDate);

        // Assert
        result.Kind.Should().Be(DateTimeKind.Utc);
        result.Should().Be(unspecifiedDate);
    }

    [Fact]
    public void ConvertFromProvider_With_UtcKind_Should_Return_SameValue()
    {
        // Arrange
        var utcDate = new DateTime(2025, 3, 22, 12, 0, 0, DateTimeKind.Utc);
        var converter = UtcDateTimeConverter.Instance;
        var convertFromProvider = converter.ConvertFromProviderExpression.Compile();

        // Act
        var result = convertFromProvider(utcDate);

        // Assert
        result.Kind.Should().Be(DateTimeKind.Utc);
        result.Should().Be(utcDate);
    }
}
