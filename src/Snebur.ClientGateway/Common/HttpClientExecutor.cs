using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using Snebur.ClientGateway.Common.Exceptions;
using Snebur.ClientGateway.Common.Factories;
using Snebur.Core.Extensions;
using Snebur.Core.Mappers;
using Snebur.SharedKernel.Abstractions;
using Microsoft.Extensions.Logging;

namespace Snebur.ClientGateway.Common;

public sealed class HttpClientExecutor : IHttpClientExecutor
{
    private static readonly SemaphoreSlim _internetStatusSyncLock = new(1, 1);

    private readonly IHttpClientResilienceOptions _resilienceOptions;
    private readonly IInternetStatusService _internetStatusService;
    private readonly IConnectionStatusNotifier _connectionStatusNotifier;
    private readonly IClientAuthorizationTokenManager _authorizationTokenManager;
    private readonly IApplicationInfo _clientApplicationInfo;
    private readonly ILogger<HttpClientExecutor> _logger;

    public HttpClientExecutor(
        IHttpClientResilienceOptions resilienceOptions,
        IInternetStatusService internetStatusService,
        IConnectionStatusNotifier connectionStatusNotifier,
        IClientAuthorizationTokenManager authorizationTokenManager,
        IApplicationInfo clientApplicationInfo,
        ILogger<HttpClientExecutor> logger)
    {
        _resilienceOptions = resilienceOptions;
        _connectionStatusNotifier = connectionStatusNotifier;
        _internetStatusService = internetStatusService;
        _authorizationTokenManager = authorizationTokenManager;
        _clientApplicationInfo = clientApplicationInfo;
        _logger = logger;
    }

    public async Task<Result<T>> SendAsync<T>(
        HttpRequestMessageFactory messageFactory,
        CancellationToken cancellationToken) where T : notnull
    {
        Guard.NotNull(messageFactory);
        try
        {
            await _resilienceOptions.ConcurrentLock.WaitAsync(cancellationToken);

            return await SendAsyncInternal<T>(messageFactory, cancellationToken);
        }
        catch (Exception ex)
        {
            var error = new UnknownError(ex,
                "HttpClientService.UnknownError",
                $"An unknown error occurred while sending the request. {ex.Message}");

            Log(messageFactory, error, ex, 0);

            return Result.Failure<T>(error);
        }
        finally
        {
            _resilienceOptions.ConcurrentLock.Release();
        }
    }

    private async Task<Result<T>> SendAsyncInternal<T>(
        HttpRequestMessageFactory messageFactory,
        CancellationToken cancellationToken,
        int attemptCount = 0) where T : notnull
    {
        Exception? exception = default;
        Error? error;

        try
        {
            var applicationName = _clientApplicationInfo.ApplicationName;
            var authorizationToken = await _authorizationTokenManager.GetAuthorizationTokenAsync();

            var httpClient = messageFactory.HttpClient;
            using var requestMessage = await messageFactory.CreateAsync(authorizationToken, applicationName);
            using var response = await httpClient.SendAsync(requestMessage, cancellationToken);

            await ValidateAuthorizationTokenAsync(response);

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        //No content is not supported, should use OperationResponse instead 

                        var noContentError = new NoContentError(
                            "HttpClientExecutor.NoContentError",
                            "The response does not contain any content.");

                        Log(messageFactory, noContentError, null, attemptCount);

                        return Result.Failure<T>(noContentError);
                    }

                    var value = await response.Content.ReadFromJsonAsync<T>(cancellationToken);
                    return Result.Success(value!);
                }
                catch (Exception ex)
                {
                    var content = await response.Content.ReadAsStringAsync(cancellationToken);
                    exception = ex;
                    error = DeserializationError.Create<T>(ex,
                        "HttpClientExecutor.DeserializationError",
                        content);
                }
            }

            var errorResponse = await GetErrorResponseAsync(messageFactory, response, attemptCount);
            if (errorResponse is not null)
            {
                var resultError = HttpErrorMapper.MapHttpStatusCodeToError(
                     response.StatusCode,
                     errorResponse.Code,
                     errorResponse.Message);

                Log(messageFactory, resultError, null, attemptCount);
                return Result.Failure<T>(resultError);
            }

            error = HttpErrorMapper.MapHttpStatusCodeToError(
                     response.StatusCode,
                     "HttpClientExecutor.HttpError",
                     "An error occurred while sending the request.");
        }
        catch (CreateHttpRequestMessageException ex)
        {
            exception = ex;
            error = new CreateHttpRequestMessageError(ex,
                "HttpClientExecutor.CreateHttpRequestMessageError",
                "An error occurred while creating the request message.");
        }
        catch (TaskCanceledException ex)
        {
            exception = ex;
            error = new TaskTimeoutError(ex,
                "HttpClientExecutor.TimeoutError",
                "The request timed out.");

        }
        catch (OperationCanceledException ex)
        {
            exception = ex;
            error = new OperationCanceledError(ex,
                "HttpClientExecutor.TimeoutError",
                "The request timed out.");
        }
        catch (HttpRequestException ex)
        {
            exception = ex;
            error = new RequestError(ex,
                $"HttpRequestException.{ex.HttpRequestError}",
                ex.Message);
        }
        catch (Exception ex)
        {
            exception = ex;
            error = new UnknownError(ex,
                "HttpClientExecutor.UnknownError",
                $"An unknown error occurred while sending the request. {ex.Message}");
        }

        await HandlerInternetConnectionAsync(cancellationToken);

        if (ShouldAbortRetry(error, attemptCount))
        {
            return Result.Failure<T>(error);
        }

        if (attemptCount >= _resilienceOptions.MaxRetryAttempts)
        {
            Log(messageFactory, error, exception, attemptCount);
            return Result.Failure<T>(error);
        }

        Log(messageFactory, error, exception, attemptCount);

        await Task.Delay(_resilienceOptions.RetryDelay, cancellationToken);
        return await SendAsyncInternal<T>(messageFactory, cancellationToken, attemptCount + 1);
    }

    private async Task ValidateAuthorizationTokenAsync(HttpResponseMessage response)
    {
        if (response.Headers.TryGetValues("Authorization", out var responseAuthHeaderValues) && responseAuthHeaderValues.Count() == 1)
        {
            await _authorizationTokenManager.ValidateAuthorizationHeaderAsync(responseAuthHeaderValues.First());
            return;
        }
        await _authorizationTokenManager.RemoveAuthorizationTokenAsync();

    }

    private async Task<ErrorResponse?> GetErrorResponseAsync(
        HttpRequestMessageFactory messageFactory,
        HttpResponseMessage response,
        int attemptCount)
    {
        if (response.Content is null)
            return null;

        try
        {
            var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            if (error != null)
            {
                return error;
            }
        }
        catch (Exception ex)
        {
            var content = await response.Content.ReadAsStringAsync();
            var json = content?.SafeTrim(1024, "[truncated]") ?? "[null]";

            var error = DeserializationError.Create<ErrorResponse>(ex,
                "HttpClientExecutor.DeserializationError",
                json);

            Log(messageFactory, error, ex, attemptCount);
        }
        return null;
    }

    private async Task HandlerInternetConnectionAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _internetStatusSyncLock.WaitAsync(cancellationToken);

            if (!await _internetStatusService.CheckInternetConnectionAsync())
            {
                await _connectionStatusNotifier.NotifyConnectionLostAsync();
                await _internetStatusService.WaitForInternetConnectionAsync();
                await _connectionStatusNotifier.NotifyConnectionRestoredAsync();
            }
        }
        finally
        {
            _internetStatusSyncLock.Release();
        }
    }

    private void Log(
        HttpRequestMessageFactory requestMessageFactory,
        Error error,
        Exception? exception,
        int attemptCount)
    {
        var logLevel = ErrorLogLevelMapper.MapErrorLevel(error);
        var requestUri = requestMessageFactory.RequestUri;
        var method = requestMessageFactory.Method;

#if DEBUG
        if (logLevel != LogLevel.Information)
        {
            Debugger.Break();
        }
#endif

        _logger.Log(
             logLevel,
             exception,
            "HTTP request. Method: {Method}, URI: {Uri}, ErrorCode: {ErrorCode}, ErrorMessage: {ErrorMessage}, AttemptCount: {AttemptCount}",
             method,
             requestUri,
             error.Code,
             error.Message,
             attemptCount);
    }

    private bool ShouldAbortRetry(Error error, int attemptCount)
    {
        return error switch
        {
            CreateHttpRequestMessageError => true,
            OperationCanceledError => true,
            DeserializationError => attemptCount >= 2,
            TaskTimeoutError => attemptCount >= 2,
            _ => false
        };
    }
     
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
