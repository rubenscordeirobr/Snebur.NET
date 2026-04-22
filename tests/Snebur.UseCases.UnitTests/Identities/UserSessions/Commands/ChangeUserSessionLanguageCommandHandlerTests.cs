using Snebur.Application.Abstractions.Services;
using Snebur.UseCases.Identities.UserSessions;
using Snebur.UseCases.Identities.UserSessions.Commands;

namespace Snebur.UseCases.UnitTests.Identities.UserSessions.Commands;

public class ChangeUserSessionLanguageCommandHandlerTests : IClassFixture<ServiceProviderMock<AnonymousRole>>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ChangeUserSessionLanguageCommand _validadeCommand;

    public ChangeUserSessionLanguageCommandHandlerTests(
        ServiceProviderMock<AnonymousRole> serviceProvider,
        ITestOutputHelper testOutput)
    {

        serviceProvider.AddTestOutput(testOutput);

        _serviceProvider = serviceProvider;

        var sessionAccessor = _serviceProvider.GetRequiredService<IHttpContextSessionAccessor>();

        _validadeCommand = new ChangeUserSessionLanguageCommand(
            sessionAccessor.UserSession_Id!.Value,
            Language.Default);
    }

    [Fact]
    public void Handler_ShouldBe_ChangeUserSessionLanguageCommandHandler()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IRequestMediator>() as IRequestMediatorTest;

        // Act
        var handlerType = mediator!.GetRequestHandler(_validadeCommand);

        // Assert
        handlerType.Should().BeOfType<ChangeUserSessionLanguageCommandHandler>();
    }

    [Fact]
    public async Task HandleAsync_ReturnsSuccess()
    {
       
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IRequestMediator>();
        // Act
        var result = await mediator.RunAsync(_validadeCommand, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeSuccessful();
    }

    [Fact]
    public async Task HandleAsync_ReturnsFailure_WhenSessionNotFound()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IRequestMediator>();
        var command = _validadeCommand with
        {
            UserSession_Id = Guid.NewGuid()
        };

        // Act
        var result = await mediator.RunAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeFailure<NotFoundError>();
    }
}
