namespace Snebur.FunctionalTests.Identities;

public partial class AdminUserServiceTests
{
    [Fact]
    public async Task GetAdminUserByIdAsync_ShouldReturnSuccess()
    {
        // Arrange
        var email = DefaultAdminUserConstants.Email;
        var getEmailResult = await _clientService.GetAdminUserByEmailAsync(email, TestContext.Current.CancellationToken);
        getEmailResult.ShouldBeSuccessful();

        var validUserId = getEmailResult.Value!.Id;

        // Act
        var result = await _clientService.GetAdminUserByIdAsync(validUserId, TestContext.Current.CancellationToken);

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
        var result = await _clientService.GetAdminUserByIdAsync(randomId, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeFailure<NotFoundError>();
    }
}

