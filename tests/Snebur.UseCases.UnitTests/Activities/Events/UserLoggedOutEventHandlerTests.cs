using Snebur.Domain.Entities.Identities.Events;

namespace Snebur.UseCases.UnitTests.Activities.Events;

public class UserLoggedOutEventHandlerTests : IClassFixture<ServiceProviderMock<AnonymousRole>>
{
    private readonly Fixture _fixture;
    private readonly IServiceProvider _serviceProvider;
    private readonly ITestOutputHelper _testOutput;

    public UserLoggedOutEventHandlerTests(
        ServiceProviderMock<AnonymousRole> serviceProviderMock,
        ITestOutputHelper testOutput)
    {
        serviceProviderMock.AddTestOutput(testOutput);
        _serviceProvider = serviceProviderMock;
        _testOutput = testOutput;
        _fixture = new Fixture();
    }

    [Fact]
    public async Task EventMediator_ShouldDispatchUserLoggedOutEvent()
    {
        // Arrange
        var session = AnonymousUserConstants.AnonymousUserSession;
        var user = _fixture.Create<TenantUser>();

        // Create a UserLoggedOutEvent with the necessary properties.
        var domainEvent = new UserLoggedOutEvent(user, "127.0.0.1");
  
        var eventMediator = _serviceProvider.GetRequiredService<IEventMediator>();

        // Create an event context including our domain event.
        var eventContext = new DomainEventContext(session, new[] { domainEvent });

        // Act
        await eventMediator.DispatchAsync(eventContext, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        eventContext
            .ShouldHaveExecutedEvent(domainEvent)
            .WithHandler<UserLoggedOutEventHandler>();
    }

    [Fact]
    public async Task HandleAsync_ShouldNotThrow()
    {
        // Arrange
        var user = _fixture.Create<TenantUser>();
        var domainEvent = new UserLoggedOutEvent(user, "127.0.0.1");

        // Create mocks/fakes for dependencies.
        var activityRepository = new ActivityRepositoryMock();
        var logger = new TestOutputLogger<UserLoggedOutEventHandler>(_testOutput);
        var userSessionAccessorMock = AnonymousUserSessionAccessorMock.CreateMock(_testOutput);

        // Create the handler.
        var handler = new UserLoggedOutEventHandler(
            activityRepository,
            userSessionAccessorMock,
            logger);

        // Act
        Func<Task> act = async () => await handler.HandleAsync(domainEvent, cancellationToken: TestContext.Current.CancellationToken);

        // Assert: Ensure that no exceptions are thrown.
        await act.Should().NotThrowAsync();
    }
}

