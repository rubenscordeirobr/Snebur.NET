namespace Snebur.Infrastructure.UnitTests.Persistence.Convertes;

public class NullableUtcDateTimeConverterTests
{
    [Fact]
    public void ConvertToProvider_With_NullValue_Should_Return_Null()
    {
        // Arrange
        var converter = NullableUtcDateTimeConverter.Instance;
        var convertToProvider = converter.ConvertToProviderExpression.Compile();

        // Act
        var result = convertToProvider(null);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ConvertToProvider_With_DefaultValue_Should_Return_Null()
    {
        // Arrange
        var defaultDateTime = default(DateTime);
        var converter = NullableUtcDateTimeConverter.Instance;
        var convertToProvider = converter.ConvertToProviderExpression.Compile();

        // Act
        var result = convertToProvider(defaultDateTime);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ConvertToProvider_With_UnspecifiedKind_Should_Return_DateTime_With_UtcKind()
    {
        // Arrange
        var unspecifiedDate = new DateTime(2025, 3, 22, 12, 0, 0, DateTimeKind.Unspecified);
        var converter = NullableUtcDateTimeConverter.Instance;
        var convertToProvider = converter.ConvertToProviderExpression.Compile();

        // Act
        var result = convertToProvider(unspecifiedDate);

        // Assert
        result.Should()
            .NotBeNull();

        result!.Value.Kind.Should().Be(DateTimeKind.Utc);

        result.Value.Should().Be(unspecifiedDate);
    }

    [Fact]
    public void ConvertToProvider_With_UtcKind_Should_Return_SameValue()
    {
        // Arrange
        var utcDate = new DateTime(2025, 3, 22, 12, 0, 0, DateTimeKind.Utc);
        var converter = NullableUtcDateTimeConverter.Instance;
        var convertToProvider = converter.ConvertToProviderExpression.Compile();

        // Act
        var result = convertToProvider(utcDate);

        // Assert
        result.Should().NotBeNull();
        result!.Value.Kind.Should().Be(DateTimeKind.Utc);
        result.Value.Should().Be(utcDate);
    }

    [Fact]
    public void ConvertFromProvider_With_NullValue_Should_Return_Null()
    {
        // Arrange
        var converter = NullableUtcDateTimeConverter.Instance;
        var convertFromProvider = converter.ConvertFromProviderExpression.Compile();

        // Act
        var result = convertFromProvider(null);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ConvertFromProvider_With_DefaultValue_Should_Return_Null()
    {
        // Arrange
        var defaultDateTime = default(DateTime);
        var converter = NullableUtcDateTimeConverter.Instance;
        var convertFromProvider = converter.ConvertFromProviderExpression.Compile();

        // Act
        var result = convertFromProvider(defaultDateTime);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ConvertFromProvider_With_UnspecifiedKind_Should_Return_DateTime_With_UtcKind()
    {
        // Arrange
        var unspecifiedDate = new DateTime(2025, 3, 22, 12, 0, 0, DateTimeKind.Unspecified);
        var converter = NullableUtcDateTimeConverter.Instance;
        var convertFromProvider = converter.ConvertFromProviderExpression.Compile();

        // Act
        var result = convertFromProvider(unspecifiedDate);

        // Assert
        result.Should().NotBeNull();
        result!.Value.Kind.Should().Be(DateTimeKind.Utc);
        result.Value.Should().Be(unspecifiedDate);
    }

    [Fact]
    public void ConvertFromProvider_With_UtcKind_Should_Return_SameValue()
    {
        // Arrange
        var utcDate = new DateTime(2025, 3, 22, 12, 0, 0, DateTimeKind.Utc);
        var converter = NullableUtcDateTimeConverter.Instance;
        var convertFromProvider = converter.ConvertFromProviderExpression.Compile();

        // Act
        var result = convertFromProvider(utcDate);

        // Assert
        result.Should().NotBeNull();
        result!.Value.Kind.Should().Be(DateTimeKind.Utc);
        result.Value.Should().Be(utcDate);
    }
}
