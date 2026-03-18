using Snebur.UseCases.Identities.Users.TenantUsers.Queries;

namespace Snebur.UseCases.UnitTests.Identities.Users.TenantUsers.Queries;

public class GetTenantUserByEmailOrPhoneNumberQueryHandlerTests : IClassFixture<ServiceProviderMock<TenantOwnerRole>>
{
    private readonly IServiceProvider _serviceProvider;

    public GetTenantUserByEmailOrPhoneNumberQueryHandlerTests(
        ServiceProviderMock<TenantOwnerRole> serviceProviderMock,
        ITestOutputHelper testOutput)
    {
        serviceProviderMock.AddTestOutput(testOutput);

        _serviceProvider = serviceProviderMock;
    }

    [Fact]
    public void Handle_ShouldBe_GetTenantUserByEmailOrPhoneNumberQueryHandler()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IRequestMediator>() as IRequestMediatorTest;
        var query = new GetTenantUserByEmailOrPhoneNumberQuery("test@example.com");

        // Act
        var handlerType = mediator!.GetRequestHandler(query);

        // Assert
        handlerType.Should().BeOfType<GetTenantUserByEmailOrPhoneNumberQueryHandler>();
    }

    [Fact]
    public async Task HandleAsync_ReturnSuccess_ByEmail()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IRequestMediator>();
        var tenantUserRepository = _serviceProvider.GetRequiredService<ITenantUserRepository>();
        var tenantUser = await tenantUserRepository.GetByEmailAsync(SystemTenantConstants.Email, TestContext.Current.CancellationToken);

        Guard.NotNull(tenantUser);

        var query = new GetTenantUserByEmailOrPhoneNumberQuery(tenantUser.Email);

        // Act
        var result = await mediator.GetAsync(query, TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        var tenantUserResponse = result.Value
            .Should()
            .BeOfType<UserResponse>()
            .Subject;

        tenantUserResponse.Email.Should().Be(tenantUser.Email);
    }

    [Fact]
    public async Task HandleAsync_ReturnSuccess_ByPhoneNumber()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IRequestMediator>();
        var tenantUserRepository = _serviceProvider.GetRequiredService<ITenantUserRepository>();
        var tenantUser = await tenantUserRepository.GetByPhoneNumberAsync(SystemTenantConstants.PhoneNumber, TestContext.Current.CancellationToken);

        Guard.NotNull(tenantUser);

        var query = new GetTenantUserByEmailOrPhoneNumberQuery(tenantUser.PhoneNumber.FullNumber);

        // Act
        var result = await mediator.GetAsync(query, TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        var tenantUserResponse = result.Value
            .Should()
            .BeOfType<UserResponse>()
            .Subject;

        tenantUserResponse.PhoneNumber.Should().Be(tenantUser.PhoneNumber);
    }

    [Fact]
    public async Task HandleAsync_ReturnFailure()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IRequestMediator>();
        var query = new GetTenantUserByEmailOrPhoneNumberQuery("nonexistent@example.com");

        // Act
        var result = await mediator.GetAsync(query, TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }
}

