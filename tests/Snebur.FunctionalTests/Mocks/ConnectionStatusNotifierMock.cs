namespace Snebur.FunctionalTests.Mocks;

public class ConnectionStatusNotifierMock : IConnectionStatusNotifier
{
    public Task NotifyConnectionFailureAsync()
    {
        return Task.CompletedTask;
    }
  
    public Task NotifyConnectionLostAsync()
    {
        // Do nothing
        return Task.CompletedTask;
    }

    public Task NotifyConnectionRestoredAsync()
    {
        // Do nothing
        return Task.CompletedTask;
    }
}
