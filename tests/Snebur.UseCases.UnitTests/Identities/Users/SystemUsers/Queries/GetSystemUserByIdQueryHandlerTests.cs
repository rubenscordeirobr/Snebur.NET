using Snebur.UseCases.Identities.Users.SystemUsers.Queries;

namespace Snebur.UseCases.UnitTests.Identities.Users.SystemUsers.Queries;

public class GetSystemUserByIdQueryHandlerTests : IClassFixture<ServiceProviderMock<AnonymousRole>>
{
    private readonly IServiceProvider _serviceProvider;

    public GetSystemUserByIdQueryHandlerTests(ServiceProviderMock<AnonymousRole> serviceProviderMock,
        ITestOutputHelper testOutput)
    {
        serviceProviderMock.AddTestOutput(testOutput);

        _serviceProvider = serviceProviderMock;
    }

    [Fact]
    public void HandleType_ShouldBe_GetSystemUserByIdQueryHandler()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IRequestMediator>() as IRequestMediatorTest;
        var query = new GetSystemUserByIdQuery(Guid.NewGuid());

        // Act
        var handlerType = mediator!.GetRequestHandler(query);

        // Assert
        handlerType.Should().BeOfType<GetSystemUserByIdQueryHandler>();
    }

    [Fact]
    public async Task HandleAsync_ReturnSuccess()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IRequestMediator>();
        var query = new GetSystemUserByIdQuery(AnonymousUserConstants.User_Id);

        // Act
        var result = await mediator.GetAsync(query, TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        var systemUser = result.Value
            .Should()
            .BeOfType<UserResponse>()
            .Subject;

        systemUser.Id.Should().Be(AnonymousUserConstants.User_Id);
    }

    [Fact]
    public async Task HandleAsync_ReturnFailure()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IRequestMediator>();
        var query = new GetSystemUserByIdQuery(Guid.NewGuid());

        // Act
        var result = await mediator.GetAsync(query, TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }
}
