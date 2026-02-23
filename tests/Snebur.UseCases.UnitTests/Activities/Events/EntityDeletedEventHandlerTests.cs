namespace Snebur.UseCases.UnitTests.Activities.Events;

public class EntityDeletedEventHandlerTests
    : IClassFixture<ServiceProviderMock<AnonymousRole>>
{
    private readonly Fixture _figure = new();
    private readonly IServiceProvider _serviceProvider;
    private readonly ITestOutputHelper _testOutput;

    public EntityDeletedEventHandlerTests(ServiceProviderMock<AnonymousRole> serviceProviderMock,
        ITestOutputHelper testOutput)
    {
        serviceProviderMock.AddTestOutput(testOutput);

        _serviceProvider = serviceProviderMock;
        _testOutput = testOutput;
    }

    [Fact]
    public async Task EventMediator_ShouldDispatch()
    {
        // Arrange
        var session = AnonymousUserConstants.AnonymousUserSession;
        var entity = _figure.Create<TenantUser>();
        var eventMediator = _serviceProvider.GetRequiredService<IEventMediator>();
        var deletedEvent = new EntityDeletedEvent<TenantUser>(entity, []);
        var eventContext = new DomainEventContext(session,[deletedEvent]);

        // Act
        await eventMediator.DispatchAsync(eventContext, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        eventContext
            .ShouldHaveExecutedEvent(deletedEvent)
            .WithHandler<EntityDeletedEventHandler<TenantUser>>();
    }

    [Fact]
    public async Task HandleAsync_ShouldNotThrow()
    {
        // Arrange
        var entity = _figure.Create<TenantUser>();
        var deletedEvent = new EntityDeletedEvent<TenantUser>(entity, []);
        var activityRepository = new ActivityRepositoryMock();
        var userSessionAccessorMock = AnonymousUserSessionAccessorMock.CreateMock(_testOutput);
        var logger = new TestOutputLogger<EntityDeletedEventHandler<TenantUser>>(_testOutput);

        var handler = new EntityDeletedEventHandler<TenantUser>(
            activityRepository,
            userSessionAccessorMock,
            logger);

        // Act
        Func<Task> task = async () => await handler.HandleAsync(deletedEvent, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        await task.Should().NotThrowAsync();
    }
}
