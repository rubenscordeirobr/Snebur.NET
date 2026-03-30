namespace Snebur.FunctionalTests.Identities;

public partial class TenantUserValidationServiceTests
{
    [Fact]
    public async Task IsEmailUnique_ShouldReturnTrue()
    {
        // Arrange
        var email = $"{Guid.NewGuid()}@example.com";

        // Act
        var result = await _clientService.IsEmailUniqueAsync(email, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.Should()
            .BeTrue();
    }
     
    [Fact]
    public async Task IsEmailUnique_ShouldReturnFalse()
    {
        // Arrange
        var email = SystemTenantConstants.Email;

        // Act
        var result = await _clientService.IsEmailUniqueAsync(email, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.Should()
            .BeFalse();
    }

    [Fact]
    public async Task IsEmailUnique_WithTenantAndOwner_ShouldReturnTrue()
    {
        // Arrange
        var email = $"{Guid.NewGuid()}@example.com";

        var currentTenantOwner_Id = SystemTenantConstants.User_Id;

        // Act
        var result = await _clientService.IsEmailUniqueAsync(
            currentTenantOwner_Id,
            email,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.Should()
            .BeTrue();
    }
}

