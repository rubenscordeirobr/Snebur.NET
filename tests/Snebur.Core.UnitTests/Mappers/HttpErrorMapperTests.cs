using System.Net;
using Snebur.Core.Mappers;

namespace Snebur.Core.UnitTests.Mappers;

public class HttpErrorMapperTests
{
    [Theory]
    [MemberData(nameof(GetHttpStatusCodeTestData))]
    public void GetHttpStatusCode_Should_Return_ExpectedStatus(Error error, HttpStatusCode expectedStatus)
    {
        // Act
        var status = HttpErrorMapper.MapErrorToHttpStatusCode(error);
        // Assert
        status.Should().Be(expectedStatus);
    }

    public static IEnumerable<object[]> GetHttpStatusCodeTestData()
    {
        yield return new object[] { new BadRequestError("BR001", "Bad request"), HttpStatusCode.BadRequest };
        yield return new object[] { new ForbiddenError("UA001", "Forbidden"), HttpStatusCode.Forbidden };
        yield return new object[] { new ValidationError("VE001", "Validation failed"), HttpStatusCode.UnprocessableContent };
        yield return new object[] { new NotFoundError("NF001", "Not found"), HttpStatusCode.NotFound };
        yield return new object[] { new DomainEventError("DE001", "Domain event conflict"), HttpStatusCode.Conflict };
        yield return new object[] { new NotImplementedError("NI001", "Not implemented"), HttpStatusCode.NotImplemented };
        yield return new object[] { new AbortedError("AB001", "Request aborted"), (HttpStatusCode)499 };
        yield return new object[] { new InternalServerError(new Exception("dummy"), "ISE001", "Internal server error"), HttpStatusCode.InternalServerError };
        yield return new object[] { new AuthenticationError("AU001", "Authentication error"), HttpStatusCode.Unauthorized };
        yield return new object[] { new TooManyRequestsError("TM001", "Too many requests"), HttpStatusCode.TooManyRequests };
    }

    [Theory]
    [InlineData(HttpStatusCode.InternalServerError, "ISE001", "Internal server error", "InternalServerError")]
    [InlineData(HttpStatusCode.BadRequest, "BR001", "Bad request", "BadRequestError")]
    [InlineData(HttpStatusCode.NotFound, "NF001", "Not found", "NotFoundError")]
    [InlineData(HttpStatusCode.UnprocessableContent, "VE001", "Validation failed", "ValidationError")]
    [InlineData(HttpStatusCode.Conflict, "DE001", "Domain event conflict", "DomainEventError")]
    [InlineData(HttpStatusCode.Unauthorized, "AU001", "Authentication error", "AuthenticationError")]
    [InlineData(HttpStatusCode.Forbidden, "UA001", "Forbidden", "ForbiddenError")]
    [InlineData(HttpStatusCode.TooManyRequests, "TM001", "Too many requests", "TooManyRequestsError")]
    public void GetErrorFromStatus_Should_Return_CorrectError_Type(
        HttpStatusCode statusCode, 
        string code, 
        string message,
        string expectedTypeName)
    {
        // Act
        var error = HttpErrorMapper.MapHttpStatusCodeToError(statusCode, code, message);
        // Assert
        error.GetType().Name.Should().Be(expectedTypeName);
        error.Code.Should().Be(code);
        error.Message.Should().Be(message);
    }

    [Fact]
    public void GetErrorFromStatus_Should_Return_AbortedError_For_ExtendedRequestAbortedStatus()
    {
        // Arrange
        var statusCode = (HttpStatusCode)ExtendedHttpStatusCode.RequestAborted;
        var code = "AB001";
        var message = "Request aborted";
        // Act
        var error = HttpErrorMapper.MapHttpStatusCodeToError(statusCode, code, message);
        // Assert
        error.Should().BeOfType<AbortedError>();
        error.Code.Should().Be(code);
        error.Message.Should().Be(message);
    }

    [Fact]
    public void GetErrorFromStatus_Should_Return_NotImplementedError_For_UnknownExtendedStatus()
    {
        // Arrange
        // Assuming that 600 is not defined in ExtendedHttpStatusCode.
        var statusCode = (HttpStatusCode)600;
        var code = "NI001";
        var message = "Not implemented extended";
        // Act
        var error = HttpErrorMapper.MapHttpStatusCodeToError(statusCode, code, message);
        // Assert
        error.Should().BeOfType<NotImplementedError>();
        error.Code.Should().StartWith("HttpStatusCode.NotImplemented.");
        error.Message.Should().Be(message);
    }
}

