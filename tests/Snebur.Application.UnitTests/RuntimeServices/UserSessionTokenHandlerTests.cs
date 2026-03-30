using Snebur.Application.Abstractions.Security;
using Snebur.SharedKernel.Models.Security;
using Microsoft.Extensions.Logging;
using Moq;

namespace Snebur.Application.UnitTests.RuntimeServices;

public class UserSessionTokenHandlerTests
{
    private readonly string _key = "SuperSecureAuthKey_ThatNeedsToBeLongEnough!";
    private readonly string _issuer = "SneburIssuer";
    private readonly string _audience = "SneburAudience";

    private readonly Mock<ISecureConfiguration> _secureConfigMock;
    private readonly Mock<ILogger<UserSessionTokenHandler>> _loggerMock;

    public UserSessionTokenHandlerTests()
    {
        _secureConfigMock = new Mock<ISecureConfiguration>();
        _loggerMock = new Mock<ILogger<UserSessionTokenHandler>>();

        _secureConfigMock.Setup(x => x.GetAuthenticationKey()).Returns(_key);
        _secureConfigMock.Setup(x => x.GetJwtIssuer()).Returns(_issuer);
        _secureConfigMock.Setup(x => x.GetJwtAudience()).Returns(_audience);
    }

    private static UserSessionClaims CreateValidClaims() => new(
        Session_Id: Guid.NewGuid(),
        Name: "John Doe",
        Email: "john@example.com",
        PhoneNumber: "1234567890",
        IsPersistent: true,
        Language: Language.Default,
        UserRole: UserRole.Admin,
        UserType: UserType.AdminUser,
        Expiration: DateTime.UtcNow.AddHours(1)
    );

    [Fact]
    public void WriteToken_ShouldGenerateValidJwt()
    {
        // Arrange
        var handler = new UserSessionTokenHandler(_secureConfigMock.Object, _loggerMock.Object);
        var claims = CreateValidClaims();

        // Act
        var token = handler.WriteToken(claims, isPersistent: true);

        // Assert
        token.Should().NotBeNullOrWhiteSpace();
        token.Split('.').Length.Should().Be(3); // JWT format
    }

    [Fact]
    public void ReadToken_ShouldReturnValidClaims()
    {
        // Arrange
        var handler = new UserSessionTokenHandler(_secureConfigMock.Object, _loggerMock.Object);
        var originalClaims = CreateValidClaims();

        var token = handler.WriteToken(originalClaims, isPersistent: false);

        // Act
        var parsedClaims = handler.ReadToken(token);

        // Assert
        parsedClaims.Should().NotBeNull();
        parsedClaims!.Name.Should().Be(originalClaims.Name);
        parsedClaims.Email.Should().Be(originalClaims.Email);
        parsedClaims.PhoneNumber.Should().Be(originalClaims.PhoneNumber);
        parsedClaims.Session_Id.Should().Be(originalClaims.Session_Id);
        parsedClaims.UserRole.Should().Be(originalClaims.UserRole);
        parsedClaims.UserType.Should().Be(originalClaims.UserType);
        parsedClaims.Expiration.Should().NotBeNull();
    }

    [Fact]
    public void ReadToken_ShouldReturnNull_WhenTokenIsMalformed()
    {
        // Arrange
        var handler = new UserSessionTokenHandler(_secureConfigMock.Object, _loggerMock.Object);

        // Act
        var result = handler.ReadToken("not.a.valid.token");

        // Assert
        result.Should().BeNull();

#pragma warning disable CS8620, CS8600, CS8604

        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object?>()),
            Times.Once);

#pragma warning restore CS8604, CS8600, CS8620
    }

    [Fact]
    public void ReadToken_ShouldReturnNull_WhenFactoryValidationFails()
    {
        // Arrange
        var handler = new UserSessionTokenHandler(_secureConfigMock.Object, _loggerMock.Object);

        // Create a claim with invalid UserType (string that won’t parse to enum)
        var invalidClaims = new UserSessionClaims(
            Session_Id: Guid.NewGuid(),
            Name: "Jane",
            Email: "jane@example.com",
            PhoneNumber: "000000000",
            IsPersistent: false,
            Language: Language.Default,
            UserRole: (UserRole)999, // Invalid role
            UserType: (UserType)999, // Invalid type
            Expiration: DateTime.UtcNow.AddHours(1)
        );

        var token = handler.WriteToken(invalidClaims, isPersistent: true);

        // Act
        var result = handler.ReadToken(token);

        // Assert
        result.Should().BeNull();

#pragma warning disable CS8620, CS8600, CS8604

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Error reading user session token")),
                null,
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object?>()),
            Times.Once);

#pragma warning restore CS8604, CS8600, CS8620
    }
}
