namespace Snebur.ClientGateway.Common.Abstractions;

public interface IHttpClientResilienceOptions
{
    int MaxRetryAttempts { get; }
    SemaphoreSlim ConcurrentLock { get; }
    TimeSpan RetryDelay { get; }
}
