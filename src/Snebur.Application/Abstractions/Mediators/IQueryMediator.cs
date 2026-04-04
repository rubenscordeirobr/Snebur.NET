namespace Snebur.Application.Abstractions.Mediators;

public interface IQueryMediator
{
    Task<Result<TResponse>> GetAsync<TResponse>(
        IQueryRequest<TResponse> query,
        CancellationToken cancellationToken = default)
            where TResponse : IResponse;

    Task<Result<IReadOnlyList<TResponse>>> GetManyAsync<TResponse>(
        IQueryRequest<TResponse> query,
        CancellationToken cancellationToken = default)
            where TResponse : IResponse;

}
