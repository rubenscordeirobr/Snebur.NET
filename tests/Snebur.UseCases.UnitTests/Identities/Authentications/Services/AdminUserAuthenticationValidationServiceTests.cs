using Snebur.UseCases.Abstractions.Identities;
using Snebur.UseCases.Identities.Authentications.Services;
using Moq;

namespace Snebur.UseCases.UnitTests.Identities.Authentications.Services;

public class AdminUserAuthenticationValidationServiceTests
{
    private readonly ITestOutputHelper _testOutput;
    private readonly Mock<IAdminUserRepository> _adminUserRepositoryMock;
    private readonly SecureConfigurationMock _secureConfigurationMock = new();
    private readonly AdminUserAuthenticationValidationService _service;

    public AdminUserAuthenticationValidationServiceTests(ITestOutputHelper testOutput)
    {
        _testOutput = testOutput;
        _adminUserRepositoryMock = new Mock<IAdminUserRepository>();
         
        _service = new AdminUserAuthenticationValidationService(
            _adminUserRepositoryMock.Object,
            _secureConfigurationMock);
    }

    [Fact]
    public void Service_ShouldBeRegistered()
    {
        // Arrange
        var serviceProviderMock = new ServiceProviderMock<AnonymousRole>();
        serviceProviderMock.AddTestOutput(_testOutput);
        // Act
        var service = serviceProviderMock.GetRequiredService<IAdminUserAuthenticationValidationService>();
        // Assert
        service.Should().BeOfType<AdminUserAuthenticationValidationService>();
    }

    [Fact]
    public async Task VerifyTenantUserCredentialsAsync_ShouldReturnFalse_WhenUserNotFound()
    {
        // Arrange
        var emailOrPhone = "test@example.com";
        var password = "password";

        _adminUserRepositoryMock
            .Setup(repo => repo.GetByEmailOrPhoneNumberAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((AdminUser)null!);

        // Act
        var result = await _service.VerifyAdminUserCredentialsAsync(emailOrPhone, password, TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task VerifyTenantUserCredentialsAsync_ShouldReturnTrue_WhenPasswordMatches()
    {
        // Arrange
        var emailOrPhone = "test@example.com";
        var passwordString = "password";
        var salt = _secureConfigurationMock.GetPasswordSalt();

        var password = Password.Create(passwordString, salt).GetRequiredValue();
        var adminUser = CreateAdminUser(password);

        _adminUserRepositoryMock
            .Setup(repo => repo.GetByEmailOrPhoneNumberAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(adminUser);
         

        // Act
        var result = await _service.VerifyAdminUserCredentialsAsync(emailOrPhone, passwordString, TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task EmailOrPhoneNumberExitsAsync_ShouldReturnExpectedResult()
    {
        // Arrange
        var emailOrPhone = "test@example.com";
        var expectedResult = true;
        _adminUserRepositoryMock
            .Setup(repo => repo.EmailOrPhoneNumberExistsAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _service.EmailOrPhoneNumberExitsAsync(emailOrPhone, TestContext.Current.CancellationToken);

        // Assert
        result.Should().Be(expectedResult);
    }

    private AdminUser CreateAdminUser(Password password)
    {
        return new AdminUser(
            name: "Admin User",
            email: "admin@example.com",
            language: Language.Default,
            role: UserRole.Admin,
            userState: UserState.New,
            userStatus: UserStatus.Online,
            phoneNumber: new PhoneNumber("+5542999990000")!,
            password: password
        );
    }
}

