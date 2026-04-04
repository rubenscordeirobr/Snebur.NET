namespace Snebur.Application.Abstractions.Handlers;

public interface IRequestHandler<TResponse> : IApplicationHandler
    where TResponse : IResponse
{
    
}

public interface IRequestHandler<TRequest, TResponse> : IRequestHandler<TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IResponse
{

}
