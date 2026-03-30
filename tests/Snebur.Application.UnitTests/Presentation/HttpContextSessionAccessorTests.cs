using Snebur.Presentation.Services;
using Snebur.SharedKernel.Models.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace Snebur.Application.UnitTests.Presentation;

public class HttpContextSessionAccessorTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void Constructor_WithNullHttpContext_ThrowsInvalidOperationException()
    {
        // Arrange
        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext)null!);
        var loggerMock = new Mock<ILogger<HttpContextSessionAccessor>>();

        // Act
        Action act = () => new HttpContextSessionAccessor(httpContextAccessorMock.Object, loggerMock.Object);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("HttpContext is not available.");
    }

    [Fact]
    public void Constructor_SetsRequestHeaderInfo_AndAuthorizationToken()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

        var loggerMock = new Mock<ILogger<HttpContextSessionAccessor>>();

        // Act
        var accessor = new HttpContextSessionAccessor(httpContextAccessorMock.Object, loggerMock.Object);

        // Assert
        accessor.RequestHeaderInfo
            .Should()
            .NotBeNull();

        accessor.AuthorizationToken.Should()
            .BeNull();
    }

    [Fact]
    public void AuthorizationToken_SetAndGetValue_WorksAsExpected()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

        var loggerMock = new Mock<ILogger<HttpContextSessionAccessor>>();

        var accessor = new HttpContextSessionAccessor(httpContextAccessorMock.Object, loggerMock.Object);

        // Act
        // Set a new token value.
        accessor.AuthorizationToken = "NewToken";

        // Assert
        accessor.AuthorizationToken.Should().Be("NewToken");

        // Verify that the underlying HttpContext.Items contains the correct value.
        context.Items.Should().ContainKey("AuthorizationToken")
            .WhoseValue.Should().Be("NewToken");
    }

    [Fact]
    public void AuthorizationToken_SetToNull_RemovesItemFromContext()
    {
        // Arrange
        var context = new DefaultHttpContext();

        // Prepopulate with a value.
        context.Items["AuthorizationToken"] = "ExistingToken";

        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

        var loggerMock = new Mock<ILogger<HttpContextSessionAccessor>>();

        var accessor = new HttpContextSessionAccessor(httpContextAccessorMock.Object, loggerMock.Object);

        // Act
        accessor.AuthorizationToken = null;

        // Assert
        accessor.AuthorizationToken.Should().BeNull();
        context.Items.Should().NotContainKey("AuthorizationToken");
    }

    [Fact]
    public void UserSessionClaims_SetAndGetValue_WorksAsExpected()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Items = new Dictionary<object, object?>();

        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

        var loggerMock = new Mock<ILogger<HttpContextSessionAccessor>>();

        var accessor = new HttpContextSessionAccessor(httpContextAccessorMock.Object, loggerMock.Object);
        var dummyClaims = _fixture.Create<UserSessionClaims>();

        // Act
        accessor.UserSessionClaims = dummyClaims;

        // Assert
        accessor.UserSessionClaims.Should().Be(dummyClaims);

        // Verify that the underlying context holds the value.
        context.Items.Should().ContainKey("UserSessionClaims")
            .WhoseValue.Should().Be(dummyClaims);
    }

    [Fact]
    public void TryGetHttpContextItem_WithInvalidType_LogsCriticalAndReturnsDefault()
    {
        // Arrange
        var context = new DefaultHttpContext();

        // Put an item with an unexpected type.
        context.Items["UserSession"] = "NotAUserSessionObject";

        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

        var loggerMock = new Mock<ILogger<HttpContextSessionAccessor>>();

        var accessor = new HttpContextSessionAccessor(httpContextAccessorMock.Object, loggerMock.Object);

        // Act
        var userSession = accessor.UserSession;

        // Assert
        userSession.Should().BeNull();

#pragma warning disable CS8620

        loggerMock.Verify(
        x => x.Log(
            It.Is<LogLevel>(l => l == LogLevel.Critical),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
        Times.AtLeastOnce);
         
#pragma warning restore CS8620 

    }
}
