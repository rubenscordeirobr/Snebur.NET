namespace Snebur.FunctionalTests.Mocks;

public class RequestErrorNotifierMock : IRequestErrorNotifier
{
    public Task NotifyRequestErrorAsync(Error error, Uri requestUri)
    {
        // Do nothing
        return Task.CompletedTask;
    }
}
