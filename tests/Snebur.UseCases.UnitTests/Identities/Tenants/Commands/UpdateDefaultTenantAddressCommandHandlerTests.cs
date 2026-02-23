using Snebur.UseCases.Identities.Tenants.Commands;

namespace Snebur.UseCases.UnitTests.Identities.Tenants.Commands;

public class UpdateDefaultTenantAddressCommandHandlerTests : IClassFixture<ServiceProviderMock<TenantOwnerRole>>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly UpdateDefaultTenantAddressCommand _validadeCommand;

    public UpdateDefaultTenantAddressCommandHandlerTests(
        ServiceProviderMock<TenantOwnerRole> serviceProvider,
        ITestOutputHelper testOutput)
    {
        serviceProvider.AddTestOutput(testOutput);

        _serviceProvider = serviceProvider;
        _validadeCommand = new UpdateDefaultTenantAddressCommand
        {
            Tenant_Id = Guid.NewGuid(),
            AddressName = "Address name",
            Address = new AddressDto
            {
                AddressName = "Valid Address Name",
                Street = "Street",
                Number = "123",
                Complement = "Complement",
                Neighborhood = "Neighborhood",
                City = "City",
                State = "PR",
                ZipCode = "8570555",
                Country = Country.Brazil
            }
        };
    }

    [Fact]
    public void Handler_ShouldBe_CreateTenantCommandHandler()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IRequestMediator>() as IRequestMediatorTest;
         
        // Act
        var handlerType = mediator!.GetRequestHandler(_validadeCommand);

        // Assert
        handlerType.Should().BeOfType<UpdateDefaultTenantAddressCommandHandler>();
    }

    [Fact]
    public async Task HandleAsync_ReturnsSuccess()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IRequestMediator>();
        var command = _validadeCommand with
        {
            Tenant_Id = SystemTenantConstants.Tenant_Id
        };

        // Act
        var result = await mediator.RunAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeSuccessful();
    }

    [Fact]
    public async Task HandleAsync_ReturnsFailure_WhenAddressNameIsInvalid()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IRequestMediator>();
        var command = _validadeCommand with
        {
            AddressName = String.Empty
        };

        // Act
        var result = await mediator.TestRunAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AddressName);
    }
}

