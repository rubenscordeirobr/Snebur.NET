namespace Snebur.UseCases.Common;

public abstract record QueryRequest<TResponse>
    : IQueryRequest<TResponse>
    where TResponse : IResponse
{
}
