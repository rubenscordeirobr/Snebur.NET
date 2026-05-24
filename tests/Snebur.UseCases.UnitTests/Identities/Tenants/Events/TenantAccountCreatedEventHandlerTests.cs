using Snebur.Domain.Entities.Identities.Events;
using Snebur.UseCases.Identities.Tenants.Events;

namespace Snebur.UseCases.UnitTests.Identities.Tenants.Events;

public class TenantAccountCreatedEventHandlerTests
     : IClassFixture<ServiceProviderMock<AnonymousRole>>
{
    private readonly Fixture _figure = new();
    private readonly IEventMediator _eventMediator;

    public TenantAccountCreatedEventHandlerTests(
        ServiceProviderMock<AnonymousRole> serviceProviderMock,
        ITestOutputHelper testOutput)
    {
        serviceProviderMock.AddTestOutput(testOutput);

        _eventMediator = serviceProviderMock.GetRequiredService<IEventMediator>();
    }

    [Fact]
    public async Task HandleAsync_ShouldLogInformation_WhenTenantCreatedEventIsHandled()
    {
        // Arrange
        var session = AnonymousUserConstants.AnonymousUserSession;
        var tenant = _figure.Create<Tenant>();
        var tenantUser = _figure.Create<TenantUser>();
        var createdEvent = new TenantCreatedEvent(tenant, tenantUser);
        var eventContext = new DomainEventContext(session, [createdEvent]);

        // Act
        await _eventMediator.DispatchAsync(eventContext, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        eventContext
            .ShouldHaveExecutedEvent(createdEvent)
            .WithHandler<TenantAccountCreatedEventHandler>();
    }
}
