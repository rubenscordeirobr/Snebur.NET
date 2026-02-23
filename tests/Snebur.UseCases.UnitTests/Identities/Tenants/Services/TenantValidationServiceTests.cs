using Snebur.UseCases.Abstractions.Identities;
using Snebur.UseCases.Identities.Tenants.Services;
using Moq;

namespace Snebur.UseCases.UnitTests.Identities.Tenants.Services;

public class TenantValidationServiceTests
{
    private readonly ITestOutputHelper _testOutput;
    private readonly Mock<ITenantRepository> _tenantRepositoryMock;
    private readonly Mock<ITenantUserRepository> _tenantUserRepositoryMock;
    private readonly TenantValidationService _validationService;
    private readonly CancellationToken _token = TestContext.Current.CancellationToken;

    public TenantValidationServiceTests(ITestOutputHelper output)
    {
        _testOutput = output;
        _tenantRepositoryMock = new Mock<ITenantRepository>();
        _tenantUserRepositoryMock = new Mock<ITenantUserRepository>();
        _validationService = new TenantValidationService(
            _tenantRepositoryMock.Object,
            _tenantUserRepositoryMock.Object);
    }

    [Fact]
    public void Service_ShouldBeRegistered()
    {
        // Arrange
        var serviceProviderMock = new ServiceProviderMock<AnonymousRole>();
        serviceProviderMock.AddTestOutput(_testOutput);

        // Act
        var service = serviceProviderMock.GetRequiredService<ITenantValidationService>();

        // Assert
        service.Should().BeOfType<TenantValidationService>();

    }
    [Fact]
    public async Task IsEmailUniqueAsync_EmailNotExists_ReturnsTrue()
    {
        // Arrange
        var email = "test@example.com";
        _tenantRepositoryMock
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
        _tenantRepositoryMock
            .Setup(repo => repo.EmailExistsAsync(email, _token))
            .ReturnsAsync(true);

        // Act
        var result = await _validationService.IsEmailUniqueAsync(email, _token);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsEmailUniqueAsync_WithTenantId_EmailNotExists_ReturnsTrue()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var email = "test@example.com";

        _tenantRepositoryMock
            .Setup(repo => repo.EmailExistsAsync(email, tenantId, _token))
            .ReturnsAsync(false);

        // Act
        var result = await _validationService.IsEmailUniqueAsync(tenantId, ownerId, email, _token);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsEmailUniqueAsync_WithTenantId_EmailExists_ReturnsFalse()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var email = "test@example.com";
        _tenantRepositoryMock
            .Setup(repo => repo.EmailExistsAsync(email, tenantId, _token))
            .ReturnsAsync(true);

        // Act
        var result = await _validationService.IsEmailUniqueAsync(tenantId, ownerId, email, _token);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsFiscalCodeUniqueAsync_FiscalCodeNotExists_ReturnsTrue()
    {
        // Arrange
        var fiscalCode = "123456789";
        _tenantRepositoryMock
            .Setup(repo => repo.FiscalCodeExistsAsync(fiscalCode, _token))
            .ReturnsAsync(false);

        // Act
        var result = await _validationService.IsFiscalCodeUniqueAsync(fiscalCode, _token);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsFiscalCodeUniqueAsync_FiscalCodeExists_ReturnsFalse()
    {
        // Arrange
        var fiscalCode = "123456789";
        _tenantRepositoryMock
            .Setup(repo => repo.FiscalCodeExistsAsync(fiscalCode, _token))
            .ReturnsAsync(true);

        // Act
        var result = await _validationService.IsFiscalCodeUniqueAsync(fiscalCode, _token);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsFiscalCodeUniqueAsync_WithTenantId_FiscalCodeNotExists_ReturnsTrue()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var fiscalCode = "123456789";
        _tenantRepositoryMock
            .Setup(repo => repo.FiscalCodeExistsAsync(fiscalCode, tenantId, _token))
            .ReturnsAsync(false);

        // Act
        var result = await _validationService.IsFiscalCodeUniqueAsync(tenantId, fiscalCode, _token);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsFiscalCodeUniqueAsync_WithTenantId_FiscalCodeExists_ReturnsFalse()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var fiscalCode = "123456789";
       
        _tenantRepositoryMock
            .Setup(repo => repo.FiscalCodeExistsAsync(fiscalCode, tenantId, _token))
            .ReturnsAsync(true);

        // Act
        var result = await _validationService.IsFiscalCodeUniqueAsync(tenantId, fiscalCode, _token);

        // Assert
        result.Should().BeFalse();
    }
}

