using Snebur.Application.Abstractions.Services;
using Snebur.Domain.Entities.Identities.Events;
using Snebur.UseCases.Identities.Authentications.Commands;

namespace Snebur.UseCases.UnitTests.Identities.Authentications.Commands;

public class AdminUserLoginCommandHandlerTests : IClassFixture<ServiceProviderMock<AnonymousRole>>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly AdminUserLoginCommand _validCommand;

    public AdminUserLoginCommandHandlerTests(
        ServiceProviderMock<AnonymousRole> serviceProviderMock,
        ITestOutputHelper testOutput)
    {
        serviceProviderMock.AddTestOutput(testOutput);

        _serviceProvider = serviceProviderMock;
        _validCommand = new AdminUserLoginCommand
        {
            EmailOrPhoneNumber = DefaultAdminUserConstants.Email,
            Password = DefaultAdminUserConstants.TestPassword,
            IsPersistent = true
        };
    }

    [Fact]
    public void Handler_ShouldBe_AdminUserLoginCommandHandler()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IRequestMediator>() as IRequestMediatorTest;

        // Act
        var handlerType = mediator!.GetRequestHandler(_validCommand);

        // Assert
        handlerType.Should().BeOfType<AdminUserLoginCommandHandler>();
    }

    [Fact]
    public async Task HandleAsync_ShouldBeSuccessful_WhenUserNotFound()
    {

        await using (var scope = _serviceProvider.CreateAsyncScope())
        {
            // Arrange
            var mediator = scope.ServiceProvider.GetRequiredService<IRequestMediator>();
            var eventMediator = (IEventMediatorTest)scope.ServiceProvider.GetRequiredService<IEventMediator>();
            var cacheSessionService = scope.ServiceProvider.GetRequiredService<IUserSessionCacheService>();

            eventMediator.CapturedEvents
                .Should()
                .BeEmpty();

            // Act
            var result = await mediator.RunAsync(_validCommand, TestContext.Current.CancellationToken);

            // Assert
            result.ShouldBeSuccessful();

            // Check if the events were captured
            eventMediator.CapturedEvents
                .Should().NotBeEmpty()
                .And.NotContain(@event => @event is TenantUserSessionStartedEvent)
                .And.Contain(@event => @event is UserSessionStartedEvent);

            eventMediator.ExecutedDomainEvents
               .Should().NotBeEmpty()
               .And.Contain(result => result.DomainEvent is UserLoggedInEvent &&
                                      result.HandlerType == typeof(UserLoggedInEventHandler));

            var userSession = eventMediator.CapturedEvents
                .OfType<UserSessionStartedEvent>()
                .First()
                .UserSession;

            userSession.CreatedAt
                .Should()
                .BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(30));

            var response = result.Value!;

            // Check if the session was cached
            var cachedUseSession = await cacheSessionService.GetSessionAsync(response.UserSession.Id, TestContext.Current.CancellationToken);
            cachedUseSession
                .Should()
                .NotBeNull();

            // Check if the session is the same as the one created
            cachedUseSession!.Id
                .Should().Be(userSession.Id);

        }
    }

    [Fact]
    public async Task HandleAsync_ShouldBeFailure_WhenUserNotFound()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IRequestMediator>();
        // Simulate user not found by using a non-existent email/phone number.
        var command = _validCommand with
        {
            EmailOrPhoneNumber = "notfound@snebur.com"
        };

        // Act
        var result = await mediator.TestRunAsync(command);

        // Assert
        result.ShouldBeFailure();
        result.Error!.Message.Should().Contain("does not exist");
    }

    [Fact]
    public async Task HandleAsync_ShouldBeFailure_WhenPasswordInvalid()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IRequestMediator>();
        var command = _validCommand with { Password = "abc" };

        // Act
        var result = await mediator.TestRunAsync(command);

        // Assert
        result.ShouldBeFailure();
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public async Task HandleAsync_ShouldBeFailure_WhenPasswordIsInvalid()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IRequestMediator>();
        var command = _validCommand with { Password = "InvalidPassword" };

        // Act
        var result = await mediator.TestRunAsync(command);

        // Assert
        result.ShouldBeFailure();
        result.Error!.Message.Should().Contain("Invalid password");
    }
}

