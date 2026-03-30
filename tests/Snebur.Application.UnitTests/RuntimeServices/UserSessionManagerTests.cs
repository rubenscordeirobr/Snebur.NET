using Snebur.SharedKernel.Models.Identities;
using Snebur.SharedKernel.Models.Security;
using Microsoft.Extensions.Logging;
using Moq;

namespace Snebur.Application.UnitTests.RuntimeServices;

public class UserSessionManagerTests
{
    private readonly Mock<IHttpContextSessionAccessor> _httpContextSessionAccessorMock;
    private readonly Mock<IUserSessionTokenHandler> _userSessionTokenHandlerMock;
    private readonly Mock<IUserSessionCacheService> _userSessionCacheServiceMock;
    private readonly Mock<ILogger<UserSessionManager>> _loggerMock;

    public UserSessionManagerTests()
    {
        _httpContextSessionAccessorMock = new Mock<IHttpContextSessionAccessor>();
        _userSessionTokenHandlerMock = new Mock<IUserSessionTokenHandler>();
        _userSessionCacheServiceMock = new Mock<IUserSessionCacheService>();
        _loggerMock = new Mock<ILogger<UserSessionManager>>();

        // Enable get/set for properties on IHttpContextSessionAccessor.
        _httpContextSessionAccessorMock.SetupAllProperties();
        // Start with a null AuthorizationToken so that the InitializeUserSessionClaims returns null.
        _httpContextSessionAccessorMock.Object.AuthorizationToken = null;
    }

    [Fact]
    public void Constructor_WithEmptyAuthorizationToken_SetsUserSessionClaimsToNull()
    {
        // Arrange: AuthorizationToken is null by default

        // Act: Creating the manager triggers the constructor logic.
        new UserSessionManager(
            _httpContextSessionAccessorMock.Object,
            _userSessionTokenHandlerMock.Object,
            _userSessionCacheServiceMock.Object,
            _loggerMock.Object);

        // Assert: UserSessionClaims remains null.
        _httpContextSessionAccessorMock.Object.UserSessionClaims.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithValidAuthorizationToken_SetsUserSessionClaims()
    {
        // Arrange
        var validToken = "validToken";
        var expectedClaims = new UserSessionClaims(
            Guid.NewGuid(),
            "John Doe",
            "john@example.com",
            "1234567890",
            IsPersistent: false,
            Language.Default,
            UserRole.Admin,
            UserType.AdminUser,
            DateTime.UtcNow.AddHours(1));

        _httpContextSessionAccessorMock.Object.AuthorizationToken = validToken;
        _userSessionTokenHandlerMock.Setup(x => x.ReadToken(validToken))
            .Returns(expectedClaims);

        // Act: Create a new instance to invoke the constructor logic.
        new UserSessionManager(
            _httpContextSessionAccessorMock.Object,
            _userSessionTokenHandlerMock.Object,
            _userSessionCacheServiceMock.Object,
            _loggerMock.Object);

        // Assert: The claims are set as expected.
        _httpContextSessionAccessorMock.Object.UserSessionClaims.Should().Be(expectedClaims);
    }

    [Fact]
    public async Task GetSessionAsync_WhenUserSessionIsSet_ReturnsUserSession()
    {
        // Arrange
        var dummySession = new Mock<IUserSession>().Object;
        _httpContextSessionAccessorMock.Object.UserSession = dummySession;

        var manager = new UserSessionManager(
            _httpContextSessionAccessorMock.Object,
            _userSessionTokenHandlerMock.Object,
            _userSessionCacheServiceMock.Object,
            _loggerMock.Object);

        // Act
        var result = await manager.GetSessionAsync();

        // Assert: It should return the already set session.
        result.Should().Be(dummySession);

        _userSessionCacheServiceMock.Verify(x => x.GetSessionAsync(It.IsAny<Guid>(), default), Times.Never);
    }

    [Fact]
    public async Task GetSessionAsync_WhenUserSessionIsNull_AndUserSessionClaimsIsSet_ReturnsCachedSession()
    {
        // Arrange
        _httpContextSessionAccessorMock.Object.UserSession = null;

        var sessionId = Guid.NewGuid();
        var dummyClaims = new UserSessionClaims(
            sessionId,
            "Jane Doe",
            "jane@example.com",
            "0987654321",
            IsPersistent: true,
            Language.Default,
            UserRole.Owner,
            UserType.TenantUser,
            DateTime.UtcNow.AddHours(1));

        _httpContextSessionAccessorMock
            .Setup(x => x.UserSessionClaims)
            .Returns(dummyClaims);

        _httpContextSessionAccessorMock
            .Setup(x => x.UserSession_Id)
            .Returns(sessionId);

        var userSessionMock = new Mock<IUserSession>();
        userSessionMock.Setup(x => x.Id)
            .Returns(sessionId);

        var cachedSession = CachedUserSession.Create(userSessionMock.Object);

        _userSessionCacheServiceMock.Setup(x => x.GetSessionAsync(sessionId, default))
            .ReturnsAsync(cachedSession);
         
        var manager = new UserSessionManager(
            _httpContextSessionAccessorMock.Object,
            _userSessionTokenHandlerMock.Object,
            _userSessionCacheServiceMock.Object,
            _loggerMock.Object);

        // Act
        var result = await manager.GetSessionAsync();

        // Assert: Cached session is returned.
        result.Should().Be(cachedSession);
        _userSessionCacheServiceMock.Verify(x => x.GetSessionAsync(sessionId, default), Times.Once);
    }

    [Fact]
    public async Task GetSessionAsync_WhenUserSessionIsNull_AndUserSessionClaimsIsNull_ReturnsNull()
    {
        // Arrange
        _httpContextSessionAccessorMock.Object.UserSession = null;
        _httpContextSessionAccessorMock.Object.UserSessionClaims = null;

        var manager = new UserSessionManager(
            _httpContextSessionAccessorMock.Object,
            _userSessionTokenHandlerMock.Object,
            _userSessionCacheServiceMock.Object,
            _loggerMock.Object);

        // Act
        var result = await manager.GetSessionAsync();

        // Assert: No session exists.
        result.Should().BeNull();
        _userSessionCacheServiceMock.Verify(x => x.GetSessionAsync(It.IsAny<Guid>(), default), Times.Never);
    }

    [Fact]
    public async Task RemoveSessionAsync_WithDifferentSessionId_DoesNotClearAuthorizationToken_ButRemovesSession()
    {
        // Arrange
        var existingSessionId = Guid.NewGuid();
        var dummyClaims = new UserSessionClaims(
            existingSessionId,
            "David",
            "david@example.com",
            "0001112222",
            IsPersistent: false,
            Language.Default,
            UserRole.Owner,
            UserType.TenantUser,
            DateTime.UtcNow.AddHours(1));

        _httpContextSessionAccessorMock.Object.UserSessionClaims = dummyClaims;
        _httpContextSessionAccessorMock.Object.AuthorizationToken = "existingToken";

        var differentSessionId = Guid.NewGuid();

        var manager = new UserSessionManager(
            _httpContextSessionAccessorMock.Object,
            _userSessionTokenHandlerMock.Object,
            _userSessionCacheServiceMock.Object,
            _loggerMock.Object);

        // Act
        await manager.RemoveSessionAsync(differentSessionId);

        // Assert: Since the IDs don’t match, the token remains.
        _httpContextSessionAccessorMock.Object.AuthorizationToken.Should().Be("existingToken");
        _userSessionCacheServiceMock.Verify(x => x.RemoveSessionAsync(differentSessionId, default), Times.Once);
    }
}

