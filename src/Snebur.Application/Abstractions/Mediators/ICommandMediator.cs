namespace Snebur.Application.Abstractions.Mediators;

public interface ICommandMediator
{
    Task<Result<TResponse>> RunAsync<TResponse>(
        ICommandRequest<TResponse> command,
        CancellationToken cancellationToken = default)
        where TResponse : IResponse;
}
