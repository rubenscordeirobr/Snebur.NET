
namespace Snebur.ClientGateway.Common;
public class HttpClientResilienceOptionsDefault : IHttpClientResilienceOptions
{
    public int MaxRetryAttempts { get; } = 3;

    public SemaphoreSlim ConcurrentLock { get; } = new SemaphoreSlim(1);

    public TimeSpan RetryDelay { get; } = TimeSpan.FromSeconds(3);
}
