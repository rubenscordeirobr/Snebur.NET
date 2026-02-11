using System.Net;

namespace Snebur.Core.UnitTests;
public class ErrorTests
{
    #region Test: Constructor Initializes Correctly

    [Theory]
    [InlineData("BadRequest", "Invalid input data.")]
    [InlineData("Unauthorized", "Access denied.")]
    [InlineData("NotFound", "Item not found.")]
    public void Constructor_ShouldInitializePropertiesCorrectly(string code, string message)
    {
        // Arrange & Act
        var error = new BadRequestError(code, message);

        // Assert
        error.Code.Should().Be(code);
        error.Message.Should().Be(message);
    }

    #endregion
     
    #region Test: Implicit String Conversion

    [Fact]
    public void ToString_ShouldReturnFormattedErrorString()
    {
        // Arrange
        var error = new BadRequestError("BadRequest", "Invalid input");

        // Act
        string result = error.ToString();

        // Assert
        result.Should().Be("BadRequest: Invalid input");
    }

    #endregion

    #region Test: StatusCode Mapping

    [Theory]
    [InlineData(typeof(BadRequestError), HttpStatusCode.BadRequest)]
    [InlineData(typeof(AuthenticationError), HttpStatusCode.Unauthorized)]
    [InlineData(typeof(ForbiddenError), HttpStatusCode.Forbidden)]
    [InlineData(typeof(ValidationError), HttpStatusCode.UnprocessableContent)]
    [InlineData(typeof(NotFoundError), HttpStatusCode.NotFound)]
    [InlineData(typeof(DomainEventError), HttpStatusCode.Conflict)]
    [InlineData(typeof(TooManyRequestsError), HttpStatusCode.TooManyRequests)]
    [InlineData(typeof(AbortedError), (HttpStatusCode)499)]
    public void StatusCode_ShouldMatchErrorType(Type errorType, HttpStatusCode expectedStatus)
    {
        // Arrange
        var error = Activator.CreateInstance(errorType, "ErrorCode", "ErrorMessage") as Error;

        // Act
        var statusCode = error!.StatusCode;

        // Assert
        statusCode.Should().Be(expectedStatus);
    }

    [Theory]
    [InlineData(typeof(InternalServerError), typeof(Exception))]
    [InlineData(typeof(DatabaseError), typeof(Exception))]
    [InlineData(typeof(OperationCanceledError), typeof(OperationCanceledException))]
    public void StatusCode_ShouldBeInternalServerError(Type errorType, Type exceptionType)
    {
        // Arrange
        var exception = Activator.CreateInstance(exceptionType) as Exception;
        var error = Activator.CreateInstance(errorType, exception, "ErrorCode", "ErrorMessage") as Error;

        // Act
        var statusCode = error!.StatusCode;

        // Assert
        statusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    #endregion
}
