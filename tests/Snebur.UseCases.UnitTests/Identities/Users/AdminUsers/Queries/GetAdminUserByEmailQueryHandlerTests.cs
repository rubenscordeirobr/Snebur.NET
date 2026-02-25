using Snebur.UseCases.Identities.Users.AdminUsers.Queries;

namespace Snebur.UseCases.UnitTests.Identities.Users.AdminUsers.Queries;

public class GetAdminUserByEmailQueryHandlerTests : IClassFixture<ServiceProviderMock<AnonymousRole>>
{
    private readonly IServiceProvider _serviceProvider;

    public GetAdminUserByEmailQueryHandlerTests(
        ServiceProviderMock<AnonymousRole> serviceProviderMock,
        ITestOutputHelper testOutput)
    {
        serviceProviderMock.AddTestOutput(testOutput);

        _serviceProvider = serviceProviderMock;
    }

    [Fact]
    public void Handle_ShouldBe_GetAdminUserByEmailQueryHandler()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IRequestMediator>() as IRequestMediatorTest;
        var query = new GetAdminUserByEmailQuery("test@example.com");

        // Act
        var handlerType = mediator!.GetRequestHandler(query);

        // Assert
        handlerType.Should().BeOfType<GetAdminUserByEmailQueryHandler>();
    }

    [Fact]
    public async Task HandleAsync_ReturnSuccess()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IRequestMediator>();
        var adminUserRepository = _serviceProvider.GetRequiredService<IAdminUserRepository>();
        var superAdminUser = await adminUserRepository.GetByEmailAsync(DefaultAdminUserConstants.Email, TestContext.Current.CancellationToken);

        Guard.NotNull(superAdminUser);

        var query = new GetAdminUserByEmailQuery(superAdminUser.Email);

        // Act
        var result = await mediator.GetAsync(query, TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        var adminUser = result.Value
            .Should()
            .BeOfType<UserResponse>()
            .Subject;

        adminUser.Email.Should().Be(superAdminUser.Email);
    }

    [Fact]
    public async Task HandleAsync_ReturnFailure()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IRequestMediator>();
        var query = new GetAdminUserByEmailQuery("nonexistent@example.com");

        // Act
        var result = await mediator.GetAsync(query, TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }
}

