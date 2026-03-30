using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Snebur.ClientGateway.Abstractions;
using Snebur.SharedKernel.Abstractions;
using Snebur.SharedKernel.Enums;
using Snebur.UseCases.Identities.Authentications.Commands;

namespace Snebur.FunctionalTests.Mocks;

public class ClientServiceProviderMock<TRoleProvider>
    : AbstractTestOutputServiceProvider, IAsyncLifetime
    where TRoleProvider : IRoleProvider, new()
{
    private readonly IdentityWebHostMock<TRoleProvider> _host;
    private readonly IServiceProvider _serviceProvider;

    protected override IServiceProvider ServiceProvider
        => _serviceProvider;

    public IServiceProvider HostServiceProvider
        => _host.Services;

    public ClientServiceProviderMock()
    {
        _host = new IdentityWebHostMock<TRoleProvider>();

        var servicesCollection = new ServiceCollection()
            .AddClientGatewayServices()
            .AddTenantUserAuthenticationServices()
            .AddAdminUserAuthenticationServices()
            .AddSingleton<ITestOutputHelper, TestOutputProxy>()
            .AddSingleton(typeof(ILogger<>), typeof(TestOutputLogger<>))
            .AddSingleton<IApplicationInfo, ClientApplicationInfoMock>()
            .AddSingleton(x => CreateHttpClientProvider())
            .AddScoped<IInternetStatusService, InternetStatusServiceMock>()
            .AddScoped<IConnectionStatusNotifier, ConnectionStatusNotifierMock>()
            .AddScoped<IRequestErrorNotifier, RequestErrorNotifierMock>()
            .AddScoped<IClientAuthorizationTokenManager, ClientAuthorizationTokenManagerMock>()
            .AddScoped<IClientTenantUserSessionContextService, ClientTenantUserSessionContextMock>()
            .AddScoped<IClientAdminUserSessionContextService, ClientAdminUserSessionContextMock>();

        _serviceProvider = servicesCollection.BuildServiceProvider();
    }

    public IHttpClientProvider CreateHttpClientProvider()
    {
        var httpClient = _host.CreateClient();
        if (Debugger.IsAttached)
        {
            httpClient.Timeout = TimeSpan.FromMinutes(20);
        }
        return new InternalHttpClientProvider(httpClient);
    }

    public async ValueTask InitializeAsync()
    {
        await _host.InitializeAsync();

        var roleProvider = new TRoleProvider();
        switch (roleProvider.UserRole)
        {
            case UserRole.Owner:
                await LoginSystemTenantOwnerAsync();
                break;
            case UserRole.Admin:
                await LoginAdminUserAsync();
                break;
            case UserRole.Anonymous:
                //do nothing
                break;
            default:
                throw new NotImplementedException($"Login for UserRole {roleProvider.UserRole} is not implemented");
        }
    }

    private async Task LoginSystemTenantOwnerAsync()
    {
        var authenticationService = _serviceProvider.GetRequiredService<ITenantUserAuthenticationService>();
        var email = SystemTenantConstants.Email;
        var testPassword = SystemTenantConstants.TestPassword;

        var command = new TenantUserLoginCommand
        {
            EmailOrPhoneNumber = email,
            Password = testPassword,
            IsPersistent = true
        };

        var result = await authenticationService.LoginAsync(command);
        if (result.IsFailure)
        {
            throw new Exception($"TenantOwner Login failed: {result.Error}");
        }
    }
    private async Task LoginAdminUserAsync()
    {
        var authenticationService = _serviceProvider.GetRequiredService<IAdminUserAuthenticationService>();
        var email = DefaultAdminUserConstants.Email;
        var testPassword = DefaultAdminUserConstants.TestPassword;

        var command = new AdminUserLoginCommand
        {
            EmailOrPhoneNumber = email,
            Password = testPassword,
            IsPersistent = true
        };

        var result = await authenticationService.LoginAsync(command);
        if (result.IsFailure)
        {
            throw new Exception($"TenantOwner Login failed: {result.Error}");
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _host.DisposeAsync();
    }

    public override void AddTestOutput(ITestOutputHelper output)
    {
        _host.AddTestOutput(output);
        base.AddTestOutput(output);
    }

    class InternalHttpClientProvider : IHttpClientProvider
    {
        private readonly HttpClient _httpClient;

        public InternalHttpClientProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public HttpClient GetHttpClient<T>()
        {
            return _httpClient;
        }
    }

}
