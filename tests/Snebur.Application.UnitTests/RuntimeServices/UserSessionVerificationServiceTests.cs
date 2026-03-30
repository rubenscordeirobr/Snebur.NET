namespace Snebur.Application.UnitTests.RuntimeServices;

public class UserSessionVerificationServiceTests :IClassFixture<ServiceProviderMock<AnonymousRole>>
{
    private readonly IServiceProvider _serviceProvider;

    public UserSessionVerificationServiceTests(
        ServiceProviderMock<AnonymousRole> serviceProvider,
        ITestOutputHelper testOutput)
    {
        serviceProvider.AddTestOutput(testOutput);

        _serviceProvider = serviceProvider;
    }
 
    [Fact]
    public async Task Verify_ShouldNotThrowException()
    {
        // Arrange
        var registrationService = _serviceProvider.GetRequiredService<IUserSessionVerificationService>();

        //Act
        Func<Task> act = registrationService.VerifyAsync;

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Verify_ShouldCreateAnonymousSession()
    {
        // Arrange
        var registrationService = (IUserSessionVerificationServiceTest)_serviceProvider.GetRequiredService<IUserSessionVerificationService>();

        //Act
        var userSession = await registrationService.CreateAnonymousSessionAsync();

        // Assert
        userSession.Should().NotBeNull();
        userSession.Should().BeOfType<UserSession>();

        userSession.Id
            .Should()
            .NotBeEmpty();

        userSession.StartedAt
            .Should()
            .BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(30));
    }
}

