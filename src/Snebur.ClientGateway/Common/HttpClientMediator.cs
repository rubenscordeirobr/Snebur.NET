using System.Runtime.CompilerServices;
using System.Text.Json;
using Snebur.ClientGateway.Common.Factories;
using Snebur.ClientGateway.Common.Helpers;
using Snebur.Core.Exceptions;
using Snebur.Core.Helpers;
using Snebur.SharedKernel.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Snebur.ClientGateway.Common;

public class HttpClientMediator<TService> : IHttpClientMediator<TService>
    where TService : ICommunicationService
{
    private readonly HttpClient _httpClient;
    private readonly ValidationResultCache _validationCache = new();
    private readonly string _baseRoute;
    private readonly JsonSerializerOptions? _jsonOptions;
    private readonly IRequestErrorNotifier _requestErrorNotifier;
    private readonly IServiceProvider _serviceProvider;

    public HttpClientMediator(
        IHttpClientProvider httpClientProvider,
        IRequestErrorNotifier requestErrorNotifier,
        IServiceProvider serviceProvider)
    {
        Guard.NotNull(httpClientProvider);

        _baseRoute = RouteBinderHelper.GetRoute<TService>();
        _jsonOptions = CommunicationServiceHelper.GetJsonOptions<TService>();
        _httpClient = httpClientProvider.GetHttpClient<TService>();
        _requestErrorNotifier = requestErrorNotifier;
        _serviceProvider = serviceProvider;
    }

    #region Queries
    public Task<Result<TResponse>> GetAsync<TResponse>(
        string? route,
        CancellationToken cancellationToken = default)
        where TResponse : notnull
    {
        var requestUri = BuildUri(route, null);
        var messageFactory = new NoContentMessageFactory(_httpClient, HttpMethod.Get, requestUri);
        return SendAsyncInternal<TResponse>(messageFactory, cancellationToken);
    }

    public Task<Result<TResponse>> GetAsync<TResponse>(
      IQueryRequest<TResponse> query,
      CancellationToken cancellationToken = default)
      where TResponse : IResponse
    {
        return GetAsyncInternal<TResponse>(null, query, cancellationToken);
    }

    public Task<Result<TResponse>> GetAsync<TResponse>(
        IQueryRequest<TResponse> query,
        string? route,
        CancellationToken cancellationToken = default)
        where TResponse : IResponse
    {
        return GetAsyncInternal<TResponse>(route, query, cancellationToken);
    }

    public Task<Result<IReadOnlyList<TResponse>>> GetManyAsync<TResponse>(
      IQueryRequest<TResponse> query,
      CancellationToken cancellationToken = default)
      where TResponse : IResponse
    {
        return GetAsyncInternal<IReadOnlyList<TResponse>>(null, query, cancellationToken);
    }

    public Task<Result<IReadOnlyList<TResponse>>> GetManyAsync<TResponse>(
        IQueryRequest<TResponse> query,
        string? route,
        CancellationToken cancellationToken = default)
        where TResponse : IResponse
    {
        return GetAsyncInternal<IReadOnlyList<TResponse>>(route, query, cancellationToken);
    }

    #endregion

    #region Commands

    // POST
    public Task<Result<TResponse>> CreateAsync<TResponse>(
      ICommandRequest<TResponse> command,
      CancellationToken cancellationToken = default)
      where TResponse : IResponse
    {
        return SendAsync(HttpMethod.Post, null, command, cancellationToken);
    }

    public Task<Result<TResponse>> PostDirectAsync<TResponse>(
        ICommandRequest<TResponse> command,
        CancellationToken cancellationToken = default)
        where TResponse : IResponse
    {
        return SendAsync(HttpMethod.Post, null, command, cancellationToken);
    }

    public Task<Result<TResponse>> PostAsync<TResponse>(
             ICommandRequest<TResponse> command,
             CancellationToken cancellationToken = default,
             [CallerMemberName] string callerMethodName = "")
             where TResponse : IResponse
    {
        var route = RouteHelper.CreateRoute(callerMethodName);
        return SendAsync(HttpMethod.Post, route, command, cancellationToken);
    }

    public Task<Result<TResponse>> PostAsync<TResponse>(
        ICommandRequest<TResponse> command,
        string? route,
        CancellationToken cancellationToken = default)
        where TResponse : IResponse
    {
        return SendAsync(HttpMethod.Post, route, command, cancellationToken);
    }

    // PUT 
    public Task<Result<TResponse>> PutAsync<TResponse>(
        ICommandRequest<TResponse> command,
        CancellationToken cancellationToken = default)
        where TResponse : IResponse
    {
        return SendAsync(HttpMethod.Put, null, command, cancellationToken);
    }

    public Task<Result<TResponse>> PutAsync<TResponse>(
        ICommandRequest<TResponse> command,
        string? route,
        CancellationToken cancellationToken = default)
        where TResponse : IResponse
    {
        return SendAsync(HttpMethod.Put, route, command, cancellationToken);
    }

    // DELETE 
    public Task<Result<TResponse>> DeleteAsync<TResponse>(
        ICommandRequest<TResponse> command,
        CancellationToken cancellationToken = default)
        where TResponse : IResponse
    {
        return SendAsync(HttpMethod.Delete, null, command, cancellationToken);
    }

    public Task<Result<TResponse>> DeleteAsync<TResponse>(
        ICommandRequest<TResponse> command,
        string? route,
        CancellationToken cancellationToken = default)
        where TResponse : IResponse
    {
        return SendAsync(HttpMethod.Delete, route, command, cancellationToken);
    }

    #endregion

    #region Validation

    public Task<bool> IsValidAsync(
       string[] parameterNames,
       object[] parameterValues,
       CancellationToken cancellationToken = default,
       [CallerMemberName] string callerMethodName = "")
    {
        var route = RouteHelper.CreateRoute(callerMethodName);
        return IsValidAsync(parameterNames, parameterValues, route, cancellationToken);
    }

    private async Task<bool> IsValidAsync(
        string[] parameterNames,
        object[] parameterValues,
        string route,
        CancellationToken cancellationToken = default)
    {
        Guard.NotNull(parameterNames);
        Guard.NotNull(parameterValues);

        var boundRoute = RouteBinderHelper.BindRoute(parameterNames, parameterValues, route);
        var keyValuePairs = HttpClientHelper.CreateFormKeyValuePairs(parameterNames, parameterValues);
        var cacheKey = CacheValidationHelper.CreateCacheKey(route, keyValuePairs);

        if (_validationCache.TryGetValue(cacheKey, out var isValid))
        {
            return isValid;
        }

        var requestUri = BuildUri(boundRoute);
        var messageFactory = new FormMessageFactory(_httpClient, HttpMethod.Post, requestUri, keyValuePairs);

        var result = await SendAsyncInternal<bool>(messageFactory, cancellationToken);
        if (result.IsSuccess)
        {
            _validationCache.Add(cacheKey, result.Value);
            return result.Value;
        }

        if (result.Error.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
#if DEBUG
            throw new BadRequestException(result.Error);
#endif
        }
        return false;
    }

    #endregion

    public Task<Result<TResponse>> FormAsync<TResponse>(
       string[] parameterNames,
       object[] parameterValues,
       CancellationToken cancellationToken,
       [CallerMemberName] string callerMethodName = "")
       where TResponse : notnull
    {
        var route = RouteHelper.CreateRoute(callerMethodName);
        return FormAsync<TResponse>(parameterNames, parameterValues, route, cancellationToken);
    }

    public Task<Result<TResponse>> FormAsync<TResponse>(
        string[] parameterNames,
        object[] parameterValues,
        string route,
        CancellationToken cancellationToken = default)
        where TResponse : notnull
    {
        Guard.NotNull(parameterNames);
        Guard.NotNull(parameterValues);

        var boundRoute = RouteBinderHelper.BindRoute(parameterNames, parameterValues, route);
        var requestUri = BuildUri(boundRoute);
        var keyValuePairs = HttpClientHelper.CreateFormKeyValuePairs(parameterNames, parameterValues);
        var messageFactory = new FormMessageFactory(_httpClient, HttpMethod.Post, requestUri, keyValuePairs);

        return SendAsyncInternal<TResponse>(messageFactory, cancellationToken);
    }

    private Task<Result<TResponse>> GetAsyncInternal<TResponse>(
        string? route,
        IQueryRequest query,
        CancellationToken cancellationToken)
        where TResponse : notnull
    {
        var boundRoute = RouteBinderHelper.BindRoute(query, route);
        var queryUri = HttpClientHelper.CreateQueryString(query);
        var requestUri = BuildUri(boundRoute, queryUri);
        var messageFactory = new NoContentMessageFactory(_httpClient, HttpMethod.Get, requestUri);

        return SendAsyncInternal<TResponse>(messageFactory, cancellationToken);
    }

    private Task<Result<TResponse>> SendAsync<TResponse>(
        HttpMethod method,
        string? routeTemplate,
        ICommandRequest<TResponse> command,
        CancellationToken cancellationToken)
        where TResponse : IResponse
    {
        var route = RouteBinderHelper.BindRoute(command, routeTemplate);
        var requestUri = BuildUri(route);

        var messageFactory = new JsonMessageFactory(
            _httpClient,
            method,
            requestUri,
            _jsonOptions,
            command);

        return SendAsyncInternal<TResponse>(messageFactory, cancellationToken);
    }

    private Uri BuildUri(string? route, string? query = null)
    {
        var baseAddress = _httpClient.BaseAddress ?? throw new InvalidOperationException(
            $"The {nameof(HttpClient)} used for the service '{typeof(TService).Name}' does not have a base address configured. " +
            "Please set the 'BaseAddress' property before making any requests.");

        var path = RouteHelper.Combine(_baseRoute, route);
        var uriBuilder = new UriBuilder(baseAddress)
        {
            Path = path,
            Query = query
        };
        return uriBuilder.Uri;
    }

    private async Task<Result<TResponse>> SendAsyncInternal<TResponse>(
        HttpRequestMessageFactory messageFactory,
        CancellationToken cancellationToken) where TResponse : notnull
    {
        try
        {
            using var executor = _serviceProvider.GetRequiredService<IHttpClientExecutor>();
            var result = await executor.SendAsync<TResponse>(messageFactory, cancellationToken);
            if (result.IsFailure)
            {
                await _requestErrorNotifier
                    .NotifyRequestErrorAsync(result.Error, messageFactory.RequestUri);

            }
            return result;
        }
        catch (Exception ex)
        {
            var message = $"Failed to send request. " +
                $"RequestUri: {messageFactory.RequestUri}," +
                $"HttpMethod: {messageFactory.Method}, " +
                $"Error: {ex.Message}";

            var error = new UnknownError(ex,
                "HttpClientMediator.SendAsyncInternal", message);

            return Result.Failure<TResponse>(error);
        }
    }
}

