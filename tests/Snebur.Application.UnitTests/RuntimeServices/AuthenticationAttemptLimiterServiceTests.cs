using System.Text.Json;
using Snebur.Application.Models.Security;
using Microsoft.Extensions.Logging;
using Moq;

namespace Snebur.Application.UnitTests.RuntimeServices;

public class AuthenticationAttemptLimiterServiceTests
{
    private readonly Mock<ICacheRepository> _cacheRepositoryMock;
    private readonly Mock<ILogger<AuthenticationAttemptLimiterService>> _loggerMock;
    private readonly AuthenticationAttemptLimiterService _service;

    public AuthenticationAttemptLimiterServiceTests()
    {
        _cacheRepositoryMock = new Mock<ICacheRepository>();
        _loggerMock = new Mock<ILogger<AuthenticationAttemptLimiterService>>();
        _service = new AuthenticationAttemptLimiterService(_cacheRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task MaxAuthenticationReachedAsync_ShouldReturnSuccess_WhenNoRecordExists()
    {
        // Arrange
        var ipAddress = "1.2.3.4";
        // Simulate cache miss
        _cacheRepositoryMock
            .Setup(x => x.StringGetAsync(It.IsAny<string>()))
            .ReturnsAsync((string?)null);

        // Act
        var result = await _service.MaxAuthenticationReachedAsync(ipAddress, TestContext.Current.CancellationToken);

        // Assert
        result.Should().Be(MaxAuthenticationResult.Success);
    }

    [Fact]
    public async Task MaxAuthenticationReachedAsync_ShouldReturnBlockedResult_WhenThresholdExceededAndWithinTimeWindow()
    {
        // Arrange
        var ipAddress = "1.2.3.4";
        var failedAttempts = 5;
        // Set last failed attempt to 1 minute ago
        var lastFailed = DateTime.UtcNow.AddMinutes(-1);
        // Create a DTO and then serialize to JSON
        var recordDto = new AuthenticationAttemptRecord(failedAttempts, lastFailed);
        
        var jsonRecord = JsonUtils.Serialize(recordDto, options: JsonSerializerOptions.Web);
        _cacheRepositoryMock
            .Setup(x => x.StringGetAsync(It.IsAny<string>()))
            .ReturnsAsync(jsonRecord);

        // Act
        var result = await _service.MaxAuthenticationReachedAsync(ipAddress, TestContext.Current.CancellationToken);

        // Assert
        result.IsMaxReached.Should().BeTrue();
        result.CurrentAttempts.Should().Be(failedAttempts);
        // The expiration should be roughly (TimeSpan.FromMinutes(failedAttempts) - (1 minute))
        var expectedExpiration = TimeSpan.FromMinutes(failedAttempts) - (DateTime.UtcNow - lastFailed);
        result.ExpirationTime?.TotalSeconds.Should().BeApproximately(expectedExpiration.TotalSeconds, 3);
    }

    [Fact]
    public async Task IncrementFailedAttemptsAsync_Should_AddNewRecord_WhenNoneExists_And_IncrementRecord_WhenItExists()
    {
        // Arrange
        var ipAddress = "1.2.3.4";
      
        var authenticationAttemptRecordSerializado = JsonUtils.Serialize(new AuthenticationAttemptRecord(1, DateTime.UtcNow.AddSeconds(-10)));
        
        _cacheRepositoryMock.SetupSequence(x => x.StringGetAsync(It.IsAny<string>()))
            .ReturnsAsync((string?)null)
            .ReturnsAsync(authenticationAttemptRecordSerializado);
            

        AuthenticationAttemptRecord? capturedRecordFirstCall = null;
        AuthenticationAttemptRecord? capturedRecordSecondCall = null;

        _cacheRepositoryMock
            .Setup(x => x.StringSetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()))
            .Callback<string, string, TimeSpan>((cacheKey, value, expiration) =>
            {
                // Deserialize the stored record for inspection
                var record = JsonUtils.Deserialize<AuthenticationAttemptRecord>(value, options: JsonSerializerOptions.Web);
                if (capturedRecordFirstCall == null)
                {
                    capturedRecordFirstCall = record;
                }
                else
                {
                    capturedRecordSecondCall = record;
                }
            })
            .Returns(Task.CompletedTask);

        // Act - First increment (record does not exist)
        await _service.IncrementFailedAttemptsAsync(ipAddress, TestContext.Current.CancellationToken);

        // Assert - First call: new record with FailedAttempts = 1
        capturedRecordFirstCall.Should().NotBeNull();
        capturedRecordFirstCall!.FailedAttempts.Should().Be(1);

        // Act - Second increment (record exists)
        await _service.IncrementFailedAttemptsAsync(ipAddress, TestContext.Current.CancellationToken);

        // Assert - Second call: record incremented to FailedAttempts = 2
        capturedRecordSecondCall.Should().NotBeNull();
        capturedRecordSecondCall!.FailedAttempts.Should().Be(2);
    }
}

public record AuthenticationAttemptRecord(
       int FailedAttempts,
       DateTime LastFailedAttempt
);

