namespace Snebur.UseCases.UnitTests.Activities.Events;

public class EntityCreatedEventHandlerTests
    : IClassFixture<ServiceProviderMock<AnonymousRole>>
{
    private readonly Fixture _figure = new();
    private readonly IServiceProvider _serviceProvider;
    private readonly ITestOutputHelper _testOutput;

    public EntityCreatedEventHandlerTests(
        ServiceProviderMock<AnonymousRole> serviceProviderMock,
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

        var createdEvent = new EntityCreatedEvent<TenantUser>(entity, []);
        var eventContext = new DomainEventContext(session, [createdEvent]);

        // Act
        await eventMediator.DispatchAsync(eventContext, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        eventContext
            .ShouldHaveExecutedEvent(createdEvent)
            .WithHandler<EntityCreatedEventHandler<TenantUser>>();
    }

    [Fact]
    public async Task HandleAsync_ShouldNotThrow()
    {
        // Arrange
        var entity = _figure.Create<TenantUser>();
        var createdEvent = new EntityCreatedEvent<TenantUser>(entity, []);
        var activityRepository = new ActivityRepositoryMock();
        var logger = new TestOutputLogger<EntityCreatedEventHandler<TenantUser>>(_testOutput);
        var userSessionAccessorMock = AnonymousUserSessionAccessorMock.CreateMock(_testOutput);

        var handler = new EntityCreatedEventHandler<TenantUser>(
            activityRepository,
            userSessionAccessorMock,
            logger);

        // Act
        Func<Task> task = async () => await handler.HandleAsync(createdEvent, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        await task.Should().NotThrowAsync();
    }
}
