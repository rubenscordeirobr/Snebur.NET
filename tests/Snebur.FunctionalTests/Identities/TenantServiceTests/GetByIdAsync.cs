namespace Snebur.FunctionalTests.Identities;

public partial class TenantServiceTests
{
    [Fact]
    public async Task GetById_ShouldReturnSuccess()
    {
        // Arrange
        var tenantId = SystemTenantConstants.Tenant_Id;
        // Act
        var result = await _clientService.GetByIdAsync(tenantId, cancellationToken: TestContext.Current.CancellationToken);
        // Assert
        result.ShouldBeSuccessful();
        result.Value!.Id.Should().Be(tenantId);
    }

    [Fact]
    public async Task GetById_ShouldReturnFailure()
    {
        // Arrange
        var randomId = Guid.NewGuid();
        // Act
        var result = await _clientService.GetByIdAsync(randomId, cancellationToken: TestContext.Current.CancellationToken);
        // Assert
        result.ShouldBeFailure<NotFoundError>();
    }
}

