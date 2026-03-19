using Snebur.UseCases.Identities.Tenants.Commands;

namespace Snebur.UseCases.UnitTests.Identities.Tenants.Commands;

public class UpdateTenantCommandHandlerTests : IClassFixture<ServiceProviderMock<TenantOwnerRole>>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly UpdateTenantCommand _validadeCommand;

    public UpdateTenantCommandHandlerTests(ServiceProviderMock<TenantOwnerRole> serviceProvider,
        ITestOutputHelper testOutput)
    {
        serviceProvider.AddTestOutput(testOutput);

        _serviceProvider = serviceProvider;
        var fakeCpf = BrazilianFakeUtils.GenerateCpf();
        _validadeCommand = new UpdateTenantCommand
        {
            Name = "Tenant name",
            FiscalCode = new FiscalCode(fakeCpf),
            Country = Country.Brazil,
            Language = Language.PortugueseBrazil,
            Currency = Currency.BRL,
            BusinessType = BusinessType.CivilRegistryOffice,
            TenantType = TenantType.Company,
            Tenant_Id = SystemTenantConstants.Tenant_Id
        };
    }

    [Fact]
    public void Handler_ShouldBe_UpdateTenantCommandHandler()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IRequestMediator>() as IRequestMediatorTest;

        // Act
        var handlerType = mediator!.GetRequestHandler(_validadeCommand);

        // Assert
        handlerType.Should().BeOfType<UpdateTenantCommandHandler>();
    }

    [Fact]
    public async Task HandleAsync_ReturnsSuccess()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IRequestMediator>();
        var command = _validadeCommand;

        // Act
        var result = await mediator.RunAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeSuccessful();
    }

    [Fact]
    public async Task HandleAsync_ReturnsNotFound_WhenTenantDoesNotExist()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IRequestMediator>();
        var command = _validadeCommand with
        {
            // Use a non-existent Tenant_Id
            Tenant_Id = Guid.NewGuid()
        };

        // Act
        var result = await mediator.RunAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeFailure<NotFoundError>();

    }
}

