
using Snebur.UseCases.Identities.Users.AdminUsers.Queries;

namespace Snebur.UseCases.UnitTests.Identities.Users.AdminUsers.Queries;

public class GetAdminUserByEmailOrPhoneNumberQueryHandlerTests : IClassFixture<ServiceProviderMock<AnonymousRole>>
{
    private readonly IServiceProvider _serviceProvider;

    public GetAdminUserByEmailOrPhoneNumberQueryHandlerTests(
        ServiceProviderMock<AnonymousRole> serviceProviderMock,
        ITestOutputHelper testOutput)
    {
        serviceProviderMock.AddTestOutput(testOutput);

        _serviceProvider = serviceProviderMock;
    }

    [Fact]
    public void Handle_ShouldBe_GetAdminUserByEmailOrPhoneNumberQueryHandler()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IRequestMediator>() as IRequestMediatorTest;
        var query = new GetAdminUserByEmailOrPhoneNumberQuery("test@example.com");

        // Act
        var handlerType = mediator!.GetRequestHandler(query);

        // Assert
        handlerType.Should().BeOfType<GetAdminUserByEmailOrPhoneNumberQueryHandler>();
    }

    [Fact]
    public async Task HandleAsync_ReturnSuccess()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IRequestMediator>();
        var adminUserRepository = _serviceProvider.GetRequiredService<IAdminUserRepository>();
        var superAdminUser = await adminUserRepository.GetByEmailAsync(DefaultAdminUserConstants.Email, TestContext.Current.CancellationToken);

        Guard.NotNull(superAdminUser);

        var query = new GetAdminUserByEmailOrPhoneNumberQuery(
            superAdminUser.Email);

        // Act
        var result = await mediator.GetAsync(query, TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        var adminUser = result.Value
            .Should()
            .BeOfType<UserResponse>()
            .Subject;

        adminUser.Email.Should().Be(superAdminUser.Email);
        adminUser.PhoneNumber.Should().Be(superAdminUser.PhoneNumber);
    }

    [Fact]
    public async Task HandleAsync_ReturnFailure()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IRequestMediator>();
        var query = new GetAdminUserByEmailOrPhoneNumberQuery("nonexistent@example.com");

        // Act
        var result = await mediator.GetAsync(query, TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }
}

