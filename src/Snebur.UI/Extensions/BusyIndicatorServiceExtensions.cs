namespace Snebur.UI.Extensions;

public static class BusyIndicatorServiceExtensions
{
    public static async Task<Task> RunWithBusyIndicatorAsync(
        this IBusyIndicatorService busyIndicatorService,
        Func<Task> operation,
        CancellationToken cancellationToken = default)
    {
        Guard.NotNull(busyIndicatorService);
        Guard.NotNull(operation);

        var reuslt = await busyIndicatorService.RunWithBusyIndicatorAsync(async () =>
        {
            await operation();
            return Result.Success(true);
        }, cancellationToken);

        if (reuslt.IsFailure)
        {
            var error = reuslt.Error;
            var errorCode = error.Code;
            var errorMessage = error.Message;

            throw new BusyIndicatorOperationException($"Error code: {errorCode}. Message: {errorMessage}");
        }
        return Task.CompletedTask;
    }
}

