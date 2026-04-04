using Snebur.Application.Abstractions.Handlers;

namespace Snebur.RuntimeServices.Abstractions;

internal interface IRequestMediatorTest
{
    IRequestHandler<TResponse> GetRequestHandler<TResponse>(
         IRequest<TResponse> request)
         where TResponse : IResponse;
}
