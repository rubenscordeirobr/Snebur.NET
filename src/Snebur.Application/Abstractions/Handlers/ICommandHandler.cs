namespace Snebur.Application.Abstractions.Handlers;

public interface ICommandHandler<TResponse> : IRequestHandler<TResponse>
    where TResponse : IResponse
{
    Task IApplicationHandler.HandleAsync(object handlerObject, CancellationToken cancellationToken )
    {
        return RunAsync((ICommandRequest<TResponse>)handlerObject, cancellationToken);
    }

    Task<Result<TResponse>> RunAsync(
        ICommandRequest<TResponse> command,
        CancellationToken cancellationToken = default);
}

internal interface ICommandHandler<TCommand, TResponse>
    : IRequestHandler<TCommand, TResponse>, ICommandHandler<TResponse>
    where TCommand : ICommandRequest<TResponse>
    where TResponse : IResponse
{
    Task<Result<TResponse>> RunAsync(
        TCommand command,
        CancellationToken cancellationToken = default);

    Task<Result<TResponse>> ICommandHandler<TResponse>.RunAsync(
        ICommandRequest<TResponse> command,
        CancellationToken cancellationToken)
    {
        return RunAsync((TCommand)command, cancellationToken);
    }
}
