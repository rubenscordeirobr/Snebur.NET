namespace Snebur.FunctionalTests.Identities;

public partial class TenantUserServiceTests
{
    [Fact]
    public async Task GetAdminUserByIdAsync_ShouldReturnSuccess()
    {
        // Arrange
        var email = SystemTenantConstants.Email;
        var getEmailResult = await _clientService.GetTenantUserByEmailAsync(email, TestContext.Current.CancellationToken);
        getEmailResult.ShouldBeSuccessful();

        var validUserId = getEmailResult.Value!.Id;

        // Act
        var result = await _clientService.GetTenantUserByIdAsync(validUserId, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeSuccessful();
        result.Value!   .Id.Should().Be(validUserId);
    }

    [Fact]
    public async Task GetAdminUserByIdAsync_ShouldReturnFailure()
    {
        // Arrange
        var randomId = Guid.NewGuid();

        // Act
        var result = await _clientService.GetTenantUserByIdAsync(randomId, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeFailure<NotFoundError>();
    }
}

