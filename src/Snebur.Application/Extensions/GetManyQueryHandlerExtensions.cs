using Snebur.Application.Abstractions.Handlers;

namespace Snebur.Application.Extensions;

public static class GetManyQueryHandlerExtensions
{
    public static Result<IReadOnlyList<TResponse>> Success<TQuery, TResponse>(
        this IGetManyQueryHandler<TQuery, TResponse> handler,
        IReadOnlyList<TResponse> list)
        where TQuery : IRequest<TResponse>
        where TResponse : IResponse
    {
        return Result.Success(list);
    }
}
