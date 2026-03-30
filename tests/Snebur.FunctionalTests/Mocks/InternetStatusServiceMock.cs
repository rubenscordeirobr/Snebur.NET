namespace Snebur.FunctionalTests.Mocks;

public class InternetStatusServiceMock: IInternetStatusService
{
    public Task<bool> CheckInternetConnectionAsync()
    {
        return Task.FromResult(true);
    }

    public Task WaitForInternetConnectionAsync()
    {
        return Task.CompletedTask;
    }
}
