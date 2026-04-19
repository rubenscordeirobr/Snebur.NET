
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Snebur.UI.Services;
public class BusyIndicatorService : IBusyIndicatorService
{
    private Func<Task>? OnBusyAsync;
    private Func<Task>? OnReleaseAsync;
    private readonly ILogger _logger;

    public BusyIndicatorService(
        ILogger<BusyIndicatorService> logger)
    {
        _logger = logger;
    }

    [MemberNotNullWhen(true, nameof(OnBusyAsync), nameof(OnReleaseAsync))]
    public bool IsInitialized { get; private set; }

    public void Initialize(Func<Task>? onBusyAsync, Func<Task>? onReleaseAsync)
    {
        Guard.NotNull(onBusyAsync);
        Guard.NotNull(onReleaseAsync);

        OnBusyAsync = onBusyAsync;
        OnReleaseAsync = onReleaseAsync;
        IsInitialized = true;
    }

    public void Busy()
    {
        ThrowIfNotInitialized();
        OnBusyAsync.Invoke();
    }

    public void Release()
    {
        ThrowIfNotInitialized();
        OnReleaseAsync.Invoke();
    }

    public async Task<Result<T>> RunWithBusyIndicatorAsync<T>(
        Func<Task<Result<T>>> operation,
        CancellationToken cancellationToken = default)
        where T : notnull
    {
        Guard.NotNull(operation);

        try
        {
            this.Busy();

            var result = await operation();

            return result;
        }
        catch (Exception ex)
        {
            var errorCode = this.GetErrorCode(nameof(RunWithBusyIndicatorAsync));
            var error = new UnknownError(ex, errorCode, ex.Message);

            _logger.LogError(ex,
                    "An error occurred while executing the operation. Error code: {ErrorCode}. Message: {Message}",
                    errorCode,
                    ex.GetNestedMessage());

            return Result.Failure<T>(error);
        }
        finally
        {
            this.Release();
        }
    }

    [MemberNotNull(nameof(OnBusyAsync), nameof(OnReleaseAsync))]
    private void ThrowIfNotInitialized()
    {
        if (!IsInitialized)
        {
            throw new InvalidOperationException(
                "The BusyIndicatorService has not been initialized. " +
                "Please, please Make sure call Busy or Release only after the service has been initialized.");
        }
    }
}
