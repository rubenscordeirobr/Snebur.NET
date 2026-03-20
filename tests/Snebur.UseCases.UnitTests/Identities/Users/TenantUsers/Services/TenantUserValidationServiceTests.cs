using Snebur.UseCases.Identities.Tenants.Services;
using Moq;

namespace Snebur.UseCases.UnitTests.Identities.Users.TenantUsers.Services;

public class TenantUserValidationServiceTests
{
    private readonly Mock<ITenantUserRepository> _tenantUserRepositoryMock;
    private readonly TenantUserValidationService _validationService;
    private readonly CancellationToken _token = TestContext.Current.CancellationToken;

    public TenantUserValidationServiceTests()
    {
        _tenantUserRepositoryMock = new Mock<ITenantUserRepository>();
        _validationService = new TenantUserValidationService(_tenantUserRepositoryMock.Object);
    }

    [Fact]
    public async Task IsEmailUniqueAsync_EmailNotExists_ReturnsTrue()
    {
        // Arrange
        var email = "test@example.com";
        _tenantUserRepositoryMock
            .Setup(repo => repo.EmailExistsAsync(email, _token))
            .ReturnsAsync(false);

        // Act
        var result = await _validationService.IsEmailUniqueAsync(email, _token);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsEmailUniqueAsync_EmailExists_ReturnsFalse()
    {
        // Arrange
        var email = "test@example.com";
        _tenantUserRepositoryMock
            .Setup(repo => repo.EmailExistsAsync(email, _token))
            .ReturnsAsync(true);

        // Act
        var result = await _validationService.IsEmailUniqueAsync(email, _token);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsEmailUniqueAsync_WithUserId_EmailNotExists_ReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "test@example.com";
        _tenantUserRepositoryMock
            .Setup(repo => repo.EmailExistsAsync(email, userId, _token))
            .ReturnsAsync(false);

        // Act
        var result = await _validationService.IsEmailUniqueAsync(userId, email, _token);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsEmailUniqueAsync_WithUserId_EmailExists_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "test@example.com";
        _tenantUserRepositoryMock
            .Setup(repo => repo.EmailExistsAsync(email, userId, _token))
            .ReturnsAsync(true);

        // Act
        var result = await _validationService.IsEmailUniqueAsync(userId, email, _token);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsPhoneNumberUniqueAsync_PhoneNotExists_ReturnsTrue()
    {
        // Arrange
        var phoneNumber = "1234567890";
        _tenantUserRepositoryMock
            .Setup(repo => repo.PhoneNumberExistsAsync(phoneNumber, _token))
            .ReturnsAsync(false);

        // Act
        var result = await _validationService.IsPhoneNumberUniqueAsync(phoneNumber, _token);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsPhoneNumberUniqueAsync_PhoneExists_ReturnsFalse()
    {
        // Arrange
        var phoneNumber = "1234567890";
        _tenantUserRepositoryMock
            .Setup(repo => repo.PhoneNumberExistsAsync(phoneNumber, _token))
            .ReturnsAsync(true);

        // Act
        var result = await _validationService.IsPhoneNumberUniqueAsync(phoneNumber, _token);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsPhoneNumberUniqueAsync_WithUserId_PhoneNotExists_ReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var phoneNumber = "1234567890";
        _tenantUserRepositoryMock
            .Setup(repo => repo.PhoneNumberExistsAsync(phoneNumber, userId, _token))
            .ReturnsAsync(false);
        
        // Act
        var result = await _validationService.IsPhoneNumberUniqueAsync(userId, phoneNumber, _token);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsPhoneNumberUniqueAsync_WithUserId_PhoneExists_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var phoneNumber = "1234567890";
        _tenantUserRepositoryMock
            .Setup(repo => repo.PhoneNumberExistsAsync(phoneNumber, userId, _token))
            .ReturnsAsync(true);

        // Act
        var result = await _validationService.IsPhoneNumberUniqueAsync(userId, phoneNumber, _token);

        // Assert
        result.Should().BeFalse();
    }
}

