using Snebur.Application.Abstractions.Handlers;
using Snebur.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace Snebur.RuntimeServices.Mediators;

public sealed class RequestMediator : IRequestMediator , IRequestMediatorTest

{
    private readonly IServiceProvider _serviceProvider;
    private readonly ICommandTrackingService _tackingService;
    private readonly ILogger<RequestMediator> _logger;

    public RequestMediator(
        IServiceProvider serviceProvider,
        ICommandTrackingService requestTrackingService,
        ILogger<RequestMediator> logger)
    {
        _serviceProvider = serviceProvider;
        _tackingService = requestTrackingService;
        _logger = logger;
    }

    #region Command

    public Task<Result<TResponse>> RunAsync<TResponse>(
         ICommandRequest<TResponse> command,
         CancellationToken cancellationToken = default)
         where TResponse : IResponse
    {
        Guard.NotNull(command);

        return RunCommandAsync(command, cancellationToken);
    }

    private async Task<Result<TResponse>> RunCommandAsync<TResponse>(
        ICommandRequest<TResponse> command,
        CancellationToken cancellationToken)
        where TResponse : IResponse
    {
        var validator = new CommandValidatorExecutor<TResponse>(
            _serviceProvider,
            _logger,
            command);

        var validationResult = await validator.ValidateAsync(cancellationToken);
        if (validationResult.IsFailure)
        {
            return Result.Failure<TResponse>(validationResult.Error);
        }

        var clientRequestId = command.ClientRequestId;
        if (await _tackingService.ExistsAsync(clientRequestId, cancellationToken))
        {
            var existingResult = await _tackingService
                .TryGetResultAsync<TResponse>(clientRequestId, cancellationToken);

            if (existingResult?.IsSuccess == true)
            {
                return Result.Success(existingResult.Value);
            }
        }

        var handler = GetRequestHandler<ICommandHandler<TResponse>, TResponse>(command);
        var result = await handler.RunAsync(command, cancellationToken);
        if (result == null)
        {
            var commandTypeName = command.GetType().GetQualifiedName();
            var message = $"Handler not found for command {commandTypeName}";

            _logger.LogError("Handler not found for command {CommandTypeName}", commandTypeName);

            throw new RequestHandlerNotFoundException(message);
        }

        if (result.IsSuccess)
        {
            await _tackingService.TrackAsync(clientRequestId, result);
        }
        return result;
    }

    #endregion

    #region Query

    public Task<Result<TResponse>> GetAsync<TResponse>(
        IQueryRequest<TResponse> query,
        CancellationToken cancellationToken = default)
        where TResponse : IResponse
    {
        Guard.NotNull(query);

        var handler = GetRequestHandler<IGetQueryResultHandler<TResponse>, TResponse>(query);
        return handler.GetAsync(query, cancellationToken);
    }

    public Task<Result<IReadOnlyList<TResponse>>> GetManyAsync<TResponse>(
        IQueryRequest<TResponse> query,
        CancellationToken cancellationToken = default)
            where TResponse : IResponse
    {
        Guard.NotNull(query);

        var handler = GetRequestHandler<IGetManyQueryHandler<TResponse>, TResponse>(query);
        return handler.GetManyAsync(query, cancellationToken);
    }

    #endregion

    #region Private

    private TRequsetHandler GetRequestHandler<TRequsetHandler, TResponse>(
          IRequest<TResponse> request)
          where TRequsetHandler : class, IRequestHandler<TResponse>
          where TResponse : IResponse
    {
        var requestType = request.GetType();
        var responseType = typeof(TResponse);

        var handlerType = typeof(IRequestHandler<,>)
            .MakeGenericType(requestType, responseType);

        var handler = _serviceProvider.GetService(handlerType);
        if (handler == null)
        {
            throw new RequestHandlerNotFoundException(
                $"Handler not found not registered for Request," +
                $" Request: {requestType.Name} and" +
                $" Response {responseType.Name} not registered" +
                $"Handler type {typeof(TRequsetHandler).Name}");
        }

        if (handler is TRequsetHandler requestHandler)
        {
            return requestHandler;
        }

        throw new InvalidCastException(
            $"Could not cast the request handler {handler.GetType().GetQualifiedName()} to {typeof(TRequsetHandler).GetQualifiedName()}");
    }

    #endregion

    #region IRequestMediatorTest

    IRequestHandler<TResponse> IRequestMediatorTest.GetRequestHandler<TResponse>(
        IRequest<TResponse> request)
    {
        return GetRequestHandler<IRequestHandler<TResponse>, TResponse>(request);
    }

    #endregion
}
