using Snebur.UseCases.Identities.Users.TenantUsers.Queries;

namespace Snebur.UseCases.UnitTests.Identities.Users.TenantUsers.Queries;

public class GetTenantUserByIdQueryHandlerTests : IClassFixture<ServiceProviderMock<TenantOwnerRole>>
{
    private readonly IServiceProvider _serviceProvider;

    public GetTenantUserByIdQueryHandlerTests(
        ServiceProviderMock<TenantOwnerRole> serviceProviderMock,
        ITestOutputHelper testOutput)
    {
        serviceProviderMock.AddTestOutput(testOutput);

        _serviceProvider = serviceProviderMock;
    }

    [Fact]
    public void HandleType_ShouldBe_GetTenantUserByIdQueryHandler()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IRequestMediator>() as IRequestMediatorTest;
        var query = new GetTenantUserByIdQuery(Guid.NewGuid());

        // Act
        var handlerType = mediator!.GetRequestHandler(query);

        // Assert
        handlerType.Should().BeOfType<GetTenantUserByIdQueryHandler>();
    }

    [Fact]
    public async Task HandleAsync_ReturnSuccess()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IRequestMediator>();

        var adminUserRepository = _serviceProvider.GetRequiredService<ITenantUserRepository>();
        var tenantUserTemp = await adminUserRepository.GetByEmailAsync(SystemTenantConstants.Email, TestContext.Current.CancellationToken);

        Guard.NotNull(tenantUserTemp);

        var query = new GetTenantUserByIdQuery(tenantUserTemp.Id);

        // Act
        var result = await mediator.GetAsync(query, TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        var tenantUser = result.Value;

        tenantUser.Should()
            .BeOfType<UserResponse>();

        tenantUser!.Id.Should().Be(tenantUser.Id);
    }

    [Fact]
    public async Task HandleAsync_ReturnFailure()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IRequestMediator>();
        var query = new GetTenantUserByIdQuery(Guid.NewGuid());

        // Act
        var result = await mediator.GetAsync(query, TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }
}

