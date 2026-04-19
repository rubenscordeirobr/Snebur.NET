using Snebur.ClientGateway.Common.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace Snebur.UI.Services;

public class InternetStatusService : IInternetStatusService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly ILogger<InternetStatusService> _logger;

    public InternetStatusService(
        ILogger<InternetStatusService> logger,
        IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
        _logger = logger;
    }

    public async Task<bool> CheckInternetConnectionAsync()
    {
        try
        {
            if (!_jsRuntime.IsJsRuntimeInitialized())
            {
                return await CheckInternetConnectionServerSideAsync();
            }

            return await _jsRuntime.InvokeAsync<bool>("eval", "navigator.onLine");
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error checking internet connection");
            return false;
        }
    }

    private Task<bool> CheckInternetConnectionServerSideAsync()
    {
        // Implement server-side logic to check internet connection
        // This is a placeholder implementation

        // In a real-world scenario, you might want to ping a known server or check a specific endpoint
        return Task.FromResult(true);
    }

    public async Task WaitForInternetConnectionAsync()
    {
        while (!await CheckInternetConnectionAsync())
        {
            await Task.Delay(1000);
        }
    }
}
