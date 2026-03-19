using Snebur.UseCases.Identities.Users.TenantUsers.Commands;

namespace Snebur.UseCases.UnitTests.Identities.Users.TenantUsers.Commands;

public class CreateTenantUserCommandHandlerTests : IClassFixture<ServiceProviderMock<TenantOwnerRole>>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly CreateTenantUserCommand _validCommand;

    public CreateTenantUserCommandHandlerTests(
        ServiceProviderMock<TenantOwnerRole> serviceProvider,
        ITestOutputHelper testOutput)
    {
        serviceProvider.AddTestOutput(testOutput);

        _serviceProvider = serviceProvider;

        var fakePhoneNumber = BrazilianFakeUtils.GenerateFakePhoneNumber();
        var fakeEmail = FakeUtils.GenerateFakeEmail();
        _validCommand = new CreateTenantUserCommand
        {
            Tenant_Id =  SystemTenantConstants.Tenant_Id,
            Name = "User FullName",
            Email = fakeEmail,
            Password = "Password123!",
            PhoneNumber = new PhoneNumber(fakePhoneNumber),
            Role = UserRole.Admin
        };
    }

    [Fact]
    public void Handler_ShouldBe_CreateTenantUserCommandHandler()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IRequestMediator>() as IRequestMediatorTest;

        // Act
        var handlerType = mediator!.GetRequestHandler(_validCommand);

        // Assert
        handlerType.Should().BeOfType<CreateTenantUserCommandHandler>();
    }

    [Fact]
    public async Task HandleAsync_ReturnsSuccess()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IRequestMediator>();
        var command = _validCommand;

        // Act
        var result = await mediator.RunAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeSuccessful();
    }

    [Fact]
    public async Task HandleAsync_ReturnsFailure_WhenTenantNotFound()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IRequestMediator>();
        // TenantId value assumed to simulate a not found tenant.
        var command = _validCommand with { Tenant_Id = Guid.Empty };

        // Act
        var result = await mediator.TestRunAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Tenant_Id);
    }

    [Fact]
    public async Task HandleAsync_ReturnsFailure_WhenPasswordIsInvalid()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IRequestMediator>();
        var command = _validCommand with { Password = "password" };

        // Act
        var result = await mediator.TestRunAsync(command);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error?.Message.Should().Contain("Password");
    }
}

