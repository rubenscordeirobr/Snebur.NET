namespace Snebur.Application.Abstractions.Handlers;

public interface IGetManyQueryHandler<TResponse> : IRequestHandler<TResponse>
    where TResponse : IResponse
{
    Task<Result<IReadOnlyList<TResponse>>> GetManyAsync(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default);

    Task IApplicationHandler.HandleAsync(object handlerObject, CancellationToken cancellationToken)
    {
        return GetManyAsync((IQueryRequest<TResponse>)handlerObject, cancellationToken);
    }
}

public interface IGetManyQueryHandler<TQuery, TResponse>
    : IRequestHandler<TQuery, TResponse>, IGetManyQueryHandler<TResponse>
    where TQuery : IRequest<TResponse>
    where TResponse : IResponse
{
    Task<Result<IReadOnlyList<TResponse>>> IGetManyQueryHandler<TResponse>.GetManyAsync(
       IRequest<TResponse> request,
       CancellationToken cancellationToken)
    {
        return GetManyAsync((TQuery)request, cancellationToken);
    }

    Task<Result<IReadOnlyList<TResponse>>> GetManyAsync(
         TQuery query,
         CancellationToken cancellationToken = default)
    {
        return HandleAsync(query, cancellationToken);
    }

    Task<Result<IReadOnlyList<TResponse>>> HandleAsync(
       TQuery query,
       CancellationToken cancellationToken = default);
 
}
