using Snebur.Domain.Entities.Identities.Events;

namespace Snebur.UseCases.UnitTests.Activities.Events;
 
public class UserLoginFailedEventHandlerTests : IClassFixture<ServiceProviderMock<AnonymousRole>>
{
    private readonly Fixture _fixture;
    private readonly IServiceProvider _serviceProvider;
    private readonly ITestOutputHelper _testOutput;

    public UserLoginFailedEventHandlerTests(
        ServiceProviderMock<AnonymousRole> serviceProviderMock,
        ITestOutputHelper testOutput)
    {
        serviceProviderMock.AddTestOutput(testOutput);
        _serviceProvider = serviceProviderMock;
        _testOutput = testOutput;
        _fixture = new Fixture();
    }

    [Fact]
    public async Task EventMediator_ShouldDispatchUserLoginFailedEvent()
    {
        // Arrange
        var session = AnonymousUserConstants.AnonymousUserSession;
        var user = _fixture.Create<TenantUser>();

        // Create a UserLoginFailedEvent using constructor initialization.
        // (Assuming a constructor: UserLoginFailedEvent(TenantUser user, string userIdentifier, string ipAddress, bool passwordFailed))
        var domainEvent = new UserLoginFailedEvent(
            user,
            "user-identifier", "failPassword",
            "127.0.0.1");

        var eventMediator = _serviceProvider.GetRequiredService<IEventMediator>();

        // Create an event context including our domain event.
        var eventContext = new DomainEventContext(session, new[] { domainEvent });

        // Act
        await eventMediator.DispatchAsync(eventContext, cancellationToken: TestContext.Current.CancellationToken);

        // Assert that the event was executed by the correct handler.
        eventContext
            .ShouldHaveExecutedEvent(domainEvent)
            .WithHandler<UserLoginFailedEventHandler>();
    }

    [Fact]
    public async Task HandleAsync_ShouldNotThrow()
    {
        // Arrange
        var user = _fixture.Create<TenantUser>();
        var domainEvent = new UserLoginFailedEvent(
          user,
          "user-identifier", "failPassword",
          "127.0.0.1");

        // Create mocks/fakes for dependencies.
        var activityRepository = new ActivityRepositoryMock();
        var logger = new TestOutputLogger<UserLoginFailedEventHandler>(_testOutput);
        var userSessionAccessorMock = AnonymousUserSessionAccessorMock.CreateMock(_testOutput);

        // Create the handler.
        var handler = new UserLoginFailedEventHandler(
            activityRepository,
            userSessionAccessorMock,
            logger);

        // Act
        Func<Task> act = async () => await handler.HandleAsync(domainEvent, cancellationToken: TestContext.Current.CancellationToken);

        // Assert: Ensure that no exceptions are thrown.
        await act.Should().NotThrowAsync();
    }
}
