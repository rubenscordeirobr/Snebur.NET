using Snebur.Core.Infos;
using Snebur.Domain.Entities.Identities.Events;
using Snebur.Domain.Entities.Identities.Factories;
using Snebur.UseCases.Identities.UserSessions.Events;

namespace Snebur.UseCases.UnitTests.Identities.UserSessions.Events;

public class UserSessionTerminatedEventHandlerTests
     : IClassFixture<ServiceProviderMock<AnonymousRole>>
{
    private readonly Fixture _fixture = new();
    private readonly IEventMediator _eventMediator;

    public UserSessionTerminatedEventHandlerTests(
        ServiceProviderMock<AnonymousRole> serviceProviderMock,
        ITestOutputHelper testOutput)
    {
        serviceProviderMock.AddTestOutput(testOutput);

        _eventMediator = serviceProviderMock.GetRequiredService<IEventMediator>();
    }

    [Fact]
    public async Task EventMediator_ShouldDispatchForUserSessionTerminatedEvent()
    {
        // Arrange
        var user = SystemTenantConstants.OwnerUser; ;
        var session = AnonymousUserConstants.AnonymousUserSession;

        var userSession = UserSessionFactory.Create(
            user: user,
            clientHeaderInfo: ClientRequestHeaderInfo.System,
            authenticationType: AuthenticationType.System,
            isPersistent: false,
            tenant_id: null);

        var createdEvent = new UserSessionTerminatedEvent(userSession, SessionTerminationReason.SessionExpired);
        var eventContext = new DomainEventContext(session, [createdEvent]);

        // Act
        await _eventMediator.DispatchAsync(eventContext, cancellationToken: TestContext.Current.CancellationToken);
        // Assert

        eventContext
            .ShouldHaveExecutedEvent(createdEvent)
            .WithHandler<UserSessionTerminatedEventHandler>();
    }
}

