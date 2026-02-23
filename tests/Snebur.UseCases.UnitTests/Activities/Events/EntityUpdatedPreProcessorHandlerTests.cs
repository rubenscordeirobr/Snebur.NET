namespace Snebur.UseCases.UnitTests.Activities.Events;

public class EntityUpdatedPreProcessorHandlerTests : IClassFixture<ServiceProviderMock<AnonymousRole>>
{
    private readonly Fixture _figure = new();
    private readonly IServiceProvider _serviceProvider;

    public EntityUpdatedPreProcessorHandlerTests(
        ServiceProviderMock<AnonymousRole> serviceProviderMock,
        ITestOutputHelper testOutput)
    {
        serviceProviderMock.AddTestOutput(testOutput);

        _serviceProvider = serviceProviderMock;
    }

    [Fact]
    public async Task EventMediator_ShouldDispatch()
    {
        // Arrange
        var session = AnonymousUserConstants.AnonymousUserSession;
        var entity = _figure.Create<TenantUser>();
        var eventMediator = _serviceProvider.GetRequiredService<IEventMediator>();
        var updateEvent = new EntityUpdatedEvent<TenantUser>(entity, []);
        var eventContext = new DomainEventContext(session, [updateEvent]);
       
        // Act
        await eventMediator.PreProcessorDispatchAsync(eventContext, TestContext.Current.CancellationToken);

        // Assert
        eventContext
            .ShouldHaveExecutedEvent(updateEvent)
            .WithHandler<EntityUpdatedPreProcessorHandler<TenantUser>>();
    }
     
    [Fact]
    public async Task PreProcessAsync_ShouldNotThrow()
    {
        // Arrange
        var session = AnonymousUserConstants.AnonymousUserSession;
        var entity = _figure.Create<TenantUser>();
        var updateEvent = new EntityUpdatedEvent<TenantUser>(entity, []);
        var eventContext = new DomainEventContext(session, [updateEvent]);
        var eventData = DomainEventDataFactory.Create(eventContext, updateEvent);
        var handler = new EntityUpdatedPreProcessorHandler<TenantUser>();

        // Act
        Func<Task> task = async () => await handler.PreProcessAsync(eventData);
      
        // Assert
        await task.Should()
            .NotThrowAsync();
    }
}
