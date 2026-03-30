using Snebur.Application.Abstractions.Security;
using Snebur.UseCases.Abstractions.Identities;
using Snebur.UseCases.Identities.Authentications.Services;
using Moq;

namespace Snebur.UseCases.UnitTests.Identities.Authentications.Services;

public class TenantUserAuthenticationValidationServiceTests
{

    private readonly Fixture _fixture = new();
    private readonly ITestOutputHelper _testOutput;
    private readonly Mock<ITenantUserRepository> _tenantUserRepositoryMock;
    private readonly Mock<ISecureConfiguration> _secureConfigurationMock;
    private readonly TenantUserAuthenticationValidationService _validationService;
    private readonly CancellationToken _token = TestContext.Current.CancellationToken;

    public TenantUserAuthenticationValidationServiceTests(ITestOutputHelper testOutput )
    {
        _testOutput = testOutput;
        _tenantUserRepositoryMock = new Mock<ITenantUserRepository>();
        _secureConfigurationMock = new Mock<ISecureConfiguration>();

        _validationService = new TenantUserAuthenticationValidationService(
            _tenantUserRepositoryMock.Object,
            _secureConfigurationMock.Object);
    }

    [Fact]
    public void Service_ShouldBeRegistered()
    {
        // Arrange
        var serviceProviderMock = new ServiceProviderMock<AnonymousRole>();
        serviceProviderMock.AddTestOutput(_testOutput);
        // Act
        var service = serviceProviderMock.GetRequiredService<ITenantUserAuthenticationValidationService>();
        // Assert
        service.Should().BeOfType<TenantUserAuthenticationValidationService>();
    }

    [Fact]
    public async Task VerifyTenantUserCredentialsAsync_UserNotFound_ReturnsFalse()
    {
        // Arrange
        var emailOrPhone = "test@example.com";
        var password = "anyPassword";
        _tenantUserRepositoryMock
            .Setup(repo => repo.GetByEmailOrPhoneNumberAsync(emailOrPhone, _token))
            .ReturnsAsync((TenantUser)null!);

        // Act
        var result = await _validationService.VerifyTenantUserCredentialsAsync(emailOrPhone, password, _token);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task VerifyTenantUserCredentialsAsync_ValidCredentials_ReturnsTrue()
    {
        // Arrange
        var emailOrPhone = "test@example.com";
        var password = "password";
        var salt = "mysalt";
        
        _secureConfigurationMock.Setup(cfg => cfg.GetPasswordSalt()).Returns(salt);
        
        var passwordInstance = Password.Create(password, salt).Value!;
        var tenantUser = CreateTenantUser(passwordInstance);

        _tenantUserRepositoryMock
            .Setup(repo => repo.GetByEmailOrPhoneNumberAsync(emailOrPhone, _token))
            .ReturnsAsync(tenantUser);

        // Act
        var result = await _validationService.VerifyTenantUserCredentialsAsync(
                emailOrPhone,
                password,
                _token);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task VerifyTenantUserCredentialsAsync_InvalidCredentials_ReturnsFalse()
    {
        // Arrange
        var emailOrPhone = "test@example.com";
        var password = "wrongPassword";
        var salt = "mysalt";
        _secureConfigurationMock.Setup(cfg => cfg.GetPasswordSalt()).Returns(salt);
      
        var passwordInstance = Password.Create("password", salt).Value!;
        var tenantUser = CreateTenantUser(passwordInstance);

        _tenantUserRepositoryMock
            .Setup(repo => repo.GetByEmailOrPhoneNumberAsync(emailOrPhone, _token))
            .ReturnsAsync(tenantUser);

        // Act
        var result = await _validationService.VerifyTenantUserCredentialsAsync(emailOrPhone, password, _token);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task EmailOrPhoneNumberExitsAsync_Exists_ReturnsTrue()
    {
        // Arrange
        var emailOrPhone = "test@example.com";
        _tenantUserRepositoryMock
            .Setup(repo => repo.EmailOrPhoneNumberExistsAsync(emailOrPhone, _token))
            .ReturnsAsync(true);

        // Act
        var result = await _validationService.EmailOrPhoneNumberExitsAsync(emailOrPhone, _token);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task EmailOrPhoneNumberExitsAsync_NotExists_ReturnsFalse()
    {
        // Arrange
        var emailOrPhone = "test@example.com";
        _tenantUserRepositoryMock
            .Setup(repo => repo.EmailOrPhoneNumberExistsAsync(emailOrPhone, _token))
            .ReturnsAsync(false);

        // Act
        var result = await _validationService.EmailOrPhoneNumberExitsAsync(emailOrPhone, _token);

        // Assert
        result.Should().BeFalse();
    }

    private TenantUser CreateTenantUser(Password password)
    {
        var tenant = _fixture.Create<Tenant>(); 
        return new TenantUser(
            tenant: tenant,
            name: "Test User",
            email: "test@example.com",
            language: Language.Default,
            role: UserRole.Anonymous,
            userState: UserState.New,
            userStatus: UserStatus.Online,
            phoneNumber: new PhoneNumber("+5542999990000")!,
            password: password
        );
    }
}
