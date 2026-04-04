using Snebur.ClientGateway.Common.Factories;

namespace Snebur.ClientGateway.Common.Abstractions;

public interface IHttpClientExecutor : IDisposable
{
    Task<Result<T>> SendAsync<T>(
        HttpRequestMessageFactory messageFactory,
        CancellationToken cancellationToken)
        where T : notnull;
}

