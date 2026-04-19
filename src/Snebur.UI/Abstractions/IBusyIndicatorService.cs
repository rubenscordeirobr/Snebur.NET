
namespace Snebur.UI.Abstractions;

public interface IBusyIndicatorService
{
    bool IsInitialized { get; }
    void Busy();
    void Release();

    void Initialize(Func<Task>? onBusyAsync, Func<Task>? onReleaseAsync);

    Task<Result<T>> RunWithBusyIndicatorAsync<T>(
        Func<Task<Result<T>>> operation,
        CancellationToken cancellationToken = default)
        where T : notnull;

}
