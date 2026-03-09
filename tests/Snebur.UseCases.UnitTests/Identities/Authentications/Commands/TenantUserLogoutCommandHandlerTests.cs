using Snebur.Application.Common;
using Snebur.Application.Abstractions.Services;
using Snebur.Domain.Entities.Identities.Events;
using Snebur.SharedKernel.Models.Identities;
using Snebur.UseCases.Identities.Authentications.Commands;
using Moq;

namespace Snebur.UseCases.UnitTests.Identities.Authentications.Commands;

public class TenantUserLogoutCommandHandlerTests : IClassFixture<ServiceProviderMock<AnonymousRole>>
{
    private readonly Mock<IIdentityUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserSessionManager> _userSessionManagerMock;
    private readonly Mock<IUserSessionRepository> _userSessionRepositoryMock;

    private readonly IServiceProvider _serviceProvider;
    private readonly TenantUserLogoutCommand _command;

    public TenantUserLogoutCommandHandlerTests(ServiceProviderMock<AnonymousRole> serviceProviderMock,
        ITestOutputHelper testOutput)
    {
        serviceProviderMock.AddTestOutput(testOutput);

        var session = AnonymousUserConstants.AnonymousUserSession;

        _serviceProvider = serviceProviderMock;
        _command = new TenantUserLogoutCommand(Guid.NewGuid());

        _unitOfWorkMock = new Mock<IIdentityUnitOfWork>();
        _userSessionManagerMock = new Mock<IUserSessionManager>();
        _userSessionRepositoryMock = new Mock<IUserSessionRepository>();

        _userSessionManagerMock
            .Setup(repo => repo.UserSession_Id)
            .Returns(_command.Session_Id);

        _unitOfWorkMock.Setup(u => u.UserSessions).Returns(_userSessionRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SaveChangesResult(new DomainEventContext(session, []), 1));
    }

    [Fact]
    public void Handler_ShouldBe_TenantUserLoginCommandHandler()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IRequestMediator>() as IRequestMediatorTest;

        // Act
        var handlerType = mediator!.GetRequestHandler(_command);

        // Assert
        handlerType.Should().BeOfType<TenantUserLogoutCommandHandler>();
    }

    [Fact]
    public async Task HandleAsync_ShouldBeSuccess()
    {
        //Arrange

        var clientSessionToken = await InitiateUserSessionAsync();

        await using (var scope = _serviceProvider.CreateAsyncScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IRequestMediator>();
            var eventMediator = (IEventMediatorTest)scope.ServiceProvider.GetRequiredService<IEventMediator>();
            var cacheSessionService = scope.ServiceProvider.GetRequiredService<IUserSessionCacheService>();

            var userSession = await cacheSessionService.GetSessionAsync(clientSessionToken, TestContext.Current.CancellationToken);

            userSession
                .Should()
                .BeOfType<CachedUserSession>();

            eventMediator.CapturedEvents.Should()
                .BeEmpty();

            var logoutCommand = new TenantUserLogoutCommand(clientSessionToken);

            // Act
            var result = await mediator.RunAsync(logoutCommand, TestContext.Current.CancellationToken);

            // Assert
            result.ShouldBeSuccessful();

            eventMediator.CapturedEvents
                .Should().NotBeEmpty()
                .And.Contain(@event => @event is UserSessionTerminatedEvent)
                .And.Contain(@event => @event is TenantUserSessionTerminatedEvent);

            eventMediator.ExecutedDomainEvents
               .Should().NotBeEmpty()
               .And.Contain(result => result.DomainEvent is UserLoggedOutEvent &&
                                      result.HandlerType == typeof(UserLoggedOutEventHandler));

            var terminatedUserSession = eventMediator.CapturedEvents
                .OfType<UserSessionTerminatedEvent>()
                .First()
                .UserSession;

            terminatedUserSession.Should()
                .BeOfType<UserSession>();

            terminatedUserSession.TerminatedAt
                .Should()
                .BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(30));

            terminatedUserSession.TerminationReason
                .Should()
                .Be(SessionTerminationReason.UserLogout);

            var userSessionRetrieved = await cacheSessionService.GetSessionAsync(clientSessionToken, TestContext.Current.CancellationToken);

            userSessionRetrieved
                .Should()
                .BeNull();
        }
    }

    private async Task<Guid> InitiateUserSessionAsync()
    {
        await using (var scope = _serviceProvider.CreateAsyncScope())
        {
            // Arrange
            var mediator = scope.ServiceProvider.GetRequiredService<IRequestMediator>();
            var eventMediator = (IEventMediatorTest)scope.ServiceProvider.GetRequiredService<IEventMediator>();

            var loginCommand = new TenantUserLoginCommand
            {
                EmailOrPhoneNumber = SystemTenantConstants.Email,
                Password = SystemTenantConstants.TestPassword,
                IsPersistent = true
            };

            var loginResult = await mediator.RunAsync(loginCommand);

            loginResult.ShouldBeSuccessful();

            var userSession = eventMediator.CapturedEvents
                .OfType<TenantUserSessionStartedEvent>()
                .Single()
                .UserSession;

            userSession.Should()
                .BeOfType<UserSession>();

            return userSession.Id;

        }
    }

   
}

