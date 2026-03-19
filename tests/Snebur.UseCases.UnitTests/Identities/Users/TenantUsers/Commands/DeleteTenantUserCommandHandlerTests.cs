using Snebur.UseCases.Identities.Users.TenantUsers.Commands;

namespace Snebur.UseCases.UnitTests.Identities.Users.TenantUsers.Commands;

public class DeleteTenantUserCommandHandlerTests : IClassFixture<ServiceProviderMock<TenantOwnerRole>>
{
    private readonly ITestOutputHelper _testOutput;
    private readonly IRequestMediator _mediator;
    private readonly DeleteTenantUserCommand _validCommand;

    public DeleteTenantUserCommandHandlerTests(
        ServiceProviderMock<TenantOwnerRole> serviceProviderMock,
        ITestOutputHelper testOutput)
    {
        serviceProviderMock.AddTestOutput(testOutput);

        _testOutput = testOutput;
        _mediator = serviceProviderMock.GetRequiredService<IRequestMediator>();
        _validCommand = new DeleteTenantUserCommand(SystemTenantConstants.User_Id);
    }

    [Fact]
    public void Handler_ShouldBe_DeleteTenantUserCommandHandler()
    {
        // Act
        var mediatorTest = (IRequestMediatorTest)_mediator;
        var handlerType = mediatorTest.GetRequestHandler(_validCommand);

        // Assert
        handlerType.Should().BeOfType<DeleteTenantUserCommandHandler>();
    }

    [Fact]
    public async Task HandleAsync_ReturnSuccess()
    {
        //// Arrange
        var serviceProvideMock = new ServiceProviderMock<TenantOwnerRole>();
        serviceProvideMock.AddTestOutput(_testOutput);

        var mediator = serviceProvideMock.GetRequiredService<IRequestMediator>();

        var fakePhoneNumber = BrazilianFakeUtils.GenerateFakePhoneNumber();
        var fakeEmail = FakeUtils.GenerateFakeEmail();
        var createUserCommand = new CreateTenantUserCommand
        {
            Tenant_Id = SystemTenantConstants.Tenant_Id,
            Name = "User FullName",
            Email = fakeEmail,
            Password = "Password123!",
            PhoneNumber = new PhoneNumber(fakePhoneNumber),
            Role = UserRole.Admin
        };

        var createUserResult = await mediator.RunAsync(createUserCommand, TestContext.Current.CancellationToken);

        createUserResult.IsSuccess
            .Should()
            .BeTrue("The user should be created successfully");
         
        var deleteCommand = new DeleteTenantUserCommand(createUserResult.Value!.Id);

        //Act
        var result = await mediator.RunAsync(deleteCommand, TestContext.Current.CancellationToken);

        //// Assert
        result.ShouldBeSuccessful();
    }

    [Fact]
    public async Task HandleAsync_ReturnNotFoundFailure()
    {
        // Arrange
        var tenantUserId = Guid.NewGuid();
        var command = new DeleteTenantUserCommand(tenantUserId);

        // Act
        var result = await _mediator.RunAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeFailure<NotFoundError>();
    }

    [Fact]
    public async Task HandleAsync_ReturnNotUnauthorizedFailure()
    {
        // Arrange
        var anonymousServiceProvider = new ServiceProviderMock<AnonymousRole>();
        anonymousServiceProvider.AddTestOutput(_testOutput);
        var mediator = anonymousServiceProvider.GetRequiredService<IRequestMediator>();

        // Act
        var result = await mediator.RunAsync(_validCommand, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeFailure<ForbiddenError>();
    }
}
