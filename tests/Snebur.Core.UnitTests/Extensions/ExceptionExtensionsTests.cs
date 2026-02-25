namespace Snebur.Core.UnitTests.Extensions;

public class ExceptionExtensionsTests
{
    [Fact]
    public void GetNestedMessage_ShouldReturnSingleMessage_WhenExceptionHasNoInnerException()
    {
        // Arrange
        var exception = new Exception("Test exception");

        // Act
        var result = exception.GetNestedMessage();

        // Assert
        result.Should().Be("Test exception");
    }

    [Fact]
    public void GetNestedMessage_ShouldReturnNestedMessages_WhenExceptionHasInnerExceptions()
    {
        // Arrange
        var innerException = new Exception("Inner exception");
        var exception = new Exception("Outer exception", innerException);

        // Act
        var result = exception.GetNestedMessage();

        // Assert
        result.Should().Be("Outer exception -> Inner exception");
    }

    [Fact]
    public void GetNestedMessage_ShouldThrowArgumentNullException_WhenExceptionIsNull()
    {
        // Arrange
        Exception? exception = null;

        // Act
        Action act = () => exception!.GetNestedMessage();

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}

