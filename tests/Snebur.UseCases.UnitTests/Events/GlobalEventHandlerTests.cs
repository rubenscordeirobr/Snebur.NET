using Snebur.Domain.Entities.Identities.Events;
using Snebur.UseCases.Events;

namespace Snebur.UseCases.UnitTests.Events;

public class GlobalEventHandlerTests
     : IClassFixture<ServiceProviderMock<AnonymousRole>>
{
    private readonly Fixture _figure = new();
    private readonly IEventMediator _eventMediator;

    public GlobalEventHandlerTests(ServiceProviderMock<AnonymousRole> serviceProviderMock,
        ITestOutputHelper testOutput)
    {
        serviceProviderMock.AddTestOutput(testOutput);

        _eventMediator = serviceProviderMock.GetRequiredService<IEventMediator>();
    }

    [Fact]
    public async Task EventMediator_ShouldDispatchForCreateEntityEvent()
    {
        // Arrange
        var session = AnonymousUserConstants.AnonymousUserSession;
        var entity = _figure.Create<TenantUser>();
        var createdEvent = new EntityCreatedEvent<TenantUser>(entity, []);
        var eventContext = new DomainEventContext(session, [createdEvent]);
       
        // Act
        await _eventMediator.DispatchAsync(eventContext, cancellationToken: TestContext.Current.CancellationToken);
        // Assert

        eventContext
            .ShouldHaveExecutedEvent(createdEvent)
            .WithHandler<GlobalEventHandler>();
    }

    [Fact]
    public async Task EventMediator_ShouldDispatchForPasswordChanged()
    {
        // Arrange
        var session = AnonymousUserConstants.AnonymousUserSession;
        var entity = _figure.Create<TenantUser>();
        var passwordChangedEvent = new PasswordChangedEvent(entity);
        var eventContext = new DomainEventContext(session, [passwordChangedEvent]);

        // Act
        await _eventMediator.DispatchAsync(eventContext, cancellationToken: TestContext.Current.CancellationToken);
        // Assert

        eventContext
            .ShouldHaveExecutedEvent(passwordChangedEvent)
            .WithHandler<GlobalEventHandler>();
    }
}

