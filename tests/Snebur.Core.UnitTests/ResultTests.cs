namespace Snebur.Core.UnitTests;

public class ResultTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void ValidationFailure_ShouldThrowArgumentNullException_WhenCodeIsNullOrEmpty(string? code)
    {
        // Act
        Action act = () => Result.ValidationFailure<string>(code!, "Error message");

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void ValidationFailure_ShouldThrowArgumentNullException_WhenErrorMessageIsNullOrEmpty(string? errorMessage)
    {
        // Act
        Action act = () => Result.ValidationFailure<string>("Error code", errorMessage!);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Success_ShouldReturnSuccessResult_WhenValueIsNotNull()
    {
        // Arrange
        var value = "Success value";

        // Act
        var result = Result.Success(value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(value);
        result.Error.Should().BeNull();
    }

    [Fact]
    public void Failure_ShouldReturnFailureResult_WhenErrorIsNotNull()
    {
        // Arrange
        var error = new ValidationError("Error code", "Error message");

        // Act
        var result = Result.Failure<string>(error);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
        result.Value.Should().BeNull();
    }

    [Fact]
    public void GetValue_ShouldThrowInvalidOperationException_WhenResultIsFailure()
    {
        // Arrange
        var error = new ValidationError("Error code", "Error message");
        var result = Result.Failure<string>(error);

        // Act
        Action act = () => result.GetRequiredValue();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot get value from failed result");
    }

    [Fact]
    public void AsFailure_ShouldThrowInvalidOperationException_WhenResultIsSuccess()
    {
        // Arrange
        var result = Result.Success("Success value");

        // Act
        Action act = () => result.AsFailure<int>();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Only failed results can be converted");
    }

    [Fact]
    public void AsFailure_ShouldReturnFailureResult_WhenResultIsFailureForClasses()
    {
        // Arrange
        var error = new ValidationError("Error code", "Error message");
        var result = Result.Failure<int>(error);
         
        // Act
        var failureResult = result.AsFailure<string>();
         
        // Assert
        failureResult.IsFailure.Should().BeTrue();
        failureResult.Error.Should().Be(error);
        failureResult.Value.Should().BeNull();
    }

    [Fact]
    public void AsFailure_ShouldReturnFailureResult_WhenResultIsFailureForStruct()
    {
        // Arrange
        var error = new ValidationError("Error code", "Error message");
        var result = Result.Failure<string>(error);

        // Act
        var failureResult = result.AsFailure<int>();

        // Assert
        failureResult.IsFailure.Should().BeTrue();
        failureResult.Error.Should().Be(error);
        failureResult.Value.Should().Be(default);
    }

    [Fact]
    public void NotFoundFailure_ShouldReturnFailureResult_WhenCalledWithValidParameters()
    {
        // Arrange
        var code = "NotFoundCode";
        var message = "Not found error message";

        // Act
        var result = Result.NotFoundFailure<string>(code, message);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<NotFoundError>();
        result.Error!.Code.Should().Be(code);
        result.Error.Message.Should().Be(message);
        result.Value.Should().BeNull();
    }

    [Fact]
    public void ConvertTo_WhenValueIsConvertible_ReturnsSuccessResult()
    {
        // Arrange: Use same type conversion (string to string)
        var originalValue = "test value";
        var result = Result.Success(originalValue); // Result<string>

        // Act
        var converted = result.ConvertTo<string>();

        // Assert
        converted.IsSuccess.Should().BeTrue();
        converted.Value.Should().Be(originalValue);
    }

    [Fact]
    public void ConvertTo_WhenValueIsNotConvertible_ThrowsInvalidOperationException()
    {
        // Arrange: string is not convertible to int
        var originalValue = "test value";
        var result = Result.Success(originalValue); // Result<string>

        // Act
        Action act = () => result.ConvertTo<int>();

        // Assert
        act.Should().Throw<InvalidOperationException>();
            
    }

    [Fact]
    public void ConvertTo_WhenResultIsFailure_ReturnsFailureResult()
    {
        // Arrange
        var error = new  InvalidOperationError ("ERR001", "Error occurred");
        var result = Result.Failure<string>(error);

        // Act
        var converted = result.ConvertTo<int>();

        // Assert
        converted.IsFailure.Should().BeTrue();
        converted.Error.Should().Be(error);
    }
     

}
