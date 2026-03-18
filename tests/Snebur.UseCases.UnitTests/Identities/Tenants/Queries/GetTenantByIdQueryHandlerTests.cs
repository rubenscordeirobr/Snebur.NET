using Snebur.UseCases.Identities.Tenants.Queries;

namespace Snebur.UseCases.UnitTests.Identities.Tenants.Queries;

public class GetTenantByIdQueryHandlerTests : IClassFixture<ServiceProviderMock<AnonymousRole>>
{
    private readonly IServiceProvider _serviceProvider;

    public GetTenantByIdQueryHandlerTests(
        ServiceProviderMock<AnonymousRole> serviceProviderMock,
        ITestOutputHelper testOutput)
    {
        serviceProviderMock.AddTestOutput(testOutput);

        _serviceProvider = serviceProviderMock;
    }

    [Fact]
    public void Handle_ShouldBe_GetTenantByIdQueryHandler()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IRequestMediator>() as IRequestMediatorTest;
        var query = new GetTenantByIdQuery(Guid.NewGuid());

        // Act
        var handlerType = mediator!.GetRequestHandler(query);

        // Assert
        handlerType.Should().BeOfType<GetTenantByIdQueryHandler>();
    }

    [Fact]
    public async Task HandleAsync_ReturnSuccess()
    {
        // Arrange
        
        await using var scope = _serviceProvider.CreateAsyncScope();

        var mediator = scope.ServiceProvider.GetRequiredService<IRequestMediator>();
        var tenantRepository = scope.ServiceProvider.GetRequiredService<ITenantRepository>();

        var systemTenant = await tenantRepository.GetByNameAsync(
            SystemTenantConstants.Name, TestContext.Current.CancellationToken);
       
        systemTenant.Should()
            .NotBeNull("because the tenant with name 'SystemTenant' should exist in the test data.");

        var query = new GetTenantByIdQuery(systemTenant!.Id);

        // Act
        var result = await mediator.GetAsync(query, TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue("because a tenant matching the given id exists");

        var tenantResponse = result.Value
            .Should()
            .BeOfType<TenantResponse>()
            .Subject;

        tenantResponse.Id.Should().Be(systemTenant.Id);
    }

    [Fact]
    public async Task HandleAsync_ReturnFailure()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IRequestMediator>();
        var query = new GetTenantByIdQuery(Guid.NewGuid());

        // Act
        var result = await mediator.GetAsync(query, TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsFailure.Should().BeTrue("because no tenant exists with the provided id");
        result.Error.Should().NotBeNull();
    }
}
