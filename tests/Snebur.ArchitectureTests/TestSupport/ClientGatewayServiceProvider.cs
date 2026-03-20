using Microsoft.Extensions.DependencyInjection;

namespace Snebur.ArchitectureTests.TestSupport;

public class ClientGatewayServiceProvider : AbstractTestOutputServiceProvider
{
    private readonly IServiceProvider _serviceProvider;
    
    public IServiceCollection Services { get; }
    protected override IServiceProvider ServiceProvider
        => _serviceProvider;

    public ClientGatewayServiceProvider()
    {
        Services = new ClientGatewayServiceCollection();
        _serviceProvider = Services.BuildServiceProvider();
    }
}
