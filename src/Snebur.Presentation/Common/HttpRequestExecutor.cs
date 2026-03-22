using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text.Json;
using Snebur.Core.Enums;
using Snebur.Core.Mappers;
using Snebur.Presentation.Common.Binders;
using Snebur.Application.Extensions;
using Snebur.SharedKernel.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Snebur.Presentation.Common;

internal sealed class HttpRequestExecutor
{
    private readonly HttpContext _httpContext;
    private readonly Type _endpointType;
    private readonly ILogger _logger;
    private readonly HttpMethodDescriptor _descriptor;
    private readonly ApiEndpointBase? _endpointInstance;
    private readonly IHttpContextSessionAccessor _httpContextSessionAccessor;

    private string EndpointName
        => _endpointType.Name;

    private CancellationToken CancellationToken
        => _httpContext.RequestAborted;

    private bool IsCancellationRequested
        => CancellationToken.IsCancellationRequested;

    internal HttpRequestExecutor(
        HttpContext httpContext,
        Type endpointType,
        HttpMethodDescriptor descriptor)
    {
        _httpContext = httpContext;
        _endpointType = endpointType;
        _descriptor = descriptor;

        var serviceProvider = httpContext.RequestServices;
        _logger = serviceProvider.GetRequiredService<ILogger<HttpRequestExecutor>>();
        _httpContextSessionAccessor = serviceProvider.GetRequiredService<IHttpContextSessionAccessor>();
        _endpointInstance = GetEndPointServiceInstance(serviceProvider);
    }

    public async Task ProcessRequestAsync()
    {
        var cancellationToken = _httpContext.RequestAborted;
        var responseResult = await GetResponseResultAsync();
        
        if (cancellationToken.IsCancellationRequested)
        {
            _httpContext.Response.StatusCode = (int)ExtendedHttpStatusCode.RequestAborted;
            return;
        }

        await WriteResponseAsync(responseResult, cancellationToken);
    }

    private async Task WriteResponseAsync(
        ResponseResult responseResult,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = _httpContext.Response;
            var authorizationHeader = JwtUtils.FormatAsAuthorizationHeader(
               _httpContextSessionAccessor.AuthorizationToken);

            response.StatusCode = responseResult.StatusCode;
            response.Headers.Authorization = authorizationHeader;

            var responseValue = responseResult.IsSuccess
                ? responseResult.Value
                : responseResult.ErrorResponse;

            if (responseValue is null)
            {
                response.StatusCode = (int)HttpStatusCode.NoContent;

                Log(LogLevel.Error,
                    null,
                    "HttpRequestExecutor.WriteResponseAsync",
                    "ResponseValue is null");

                return;
            }

            var jsonOptions = GetJsonSerializerOptions();

            await _httpContext.Response.WriteAsJsonAsync(
                responseValue,
                responseValue.GetType(), jsonOptions,
                cancellationToken);
        }
        catch (Exception ex)
        {
            Log(logLevel: LogLevel.Warning,
                exception: ex,
                errorCode: "HttpRequestExecutor.WriteResponseAsync",
                errorMessage: ex.GetNestedMessage());
        }
    }
     
    public async Task<ResponseResult> GetResponseResultAsync()
    {
        if (_endpointInstance is null)
        {
            return ResponseResult.Error(
                HttpStatusCode.InternalServerError,
                "HttpRequestExecutor.InvalidEndPointType",
                $"Type {EndpointName} is not found");
        }

        _httpContextSessionAccessor.EndpointInstance = _endpointInstance;

        if (_httpContextSessionAccessor.GetRequiredUserSession().IsAnonymous() &&
            !_descriptor.IsAllowAnonymous)
        {
            Log(LogLevel.Warning, null, "Anonymous user is not allowed to access the endpoint {EndpointName}", EndpointName);

            return ResponseResult.Error(
                HttpStatusCode.Unauthorized,
                "HttpRequestExecutor.AnonymousAccessDenied",
                $"Anonymous user is not allowed to access the endpoint {EndpointName}");
        }

        try
        {
            return await GetResponseAsync();
        }
        catch (OperationCanceledException ex)
        {
            Log(LogLevel.Information, ex, "HttpRequestExecutor.RequestCancelled", ex.GetNestedMessage());
            return ResponseResult.Error(ExtendedHttpStatusCode.RequestAborted,
                "HttpRequestExecutor.RequestCancelled",
                "Request was canceled");
        }
        catch (Exception ex)
        {
            if (IsCancellationRequested)
            {
                Log(LogLevel.Information, ex, "HttpRequestExecutor.RequestCancelled", ex.GetNestedMessage());

                return ResponseResult.Error(ExtendedHttpStatusCode.RequestAborted,
                    "HttpRequestExecutor.RequestCancelled",
                    "Request was canceled");
            }

            var error = HttpErrorMapper.MapExceptionToError(ex,
                "HttpRequestExecutor.ProcessRequestAsync");

            Log(error, ex);

            return ResponseResult.Error(error);

        }
    }
    private ApiEndpointBase? GetEndPointServiceInstance(IServiceProvider serviceProvider)
    {
        var endpointInstance = CreateEndpointServiceInstance(serviceProvider);
        if (endpointInstance is not ApiEndpointBase endPoint)
        {
            _logger.LogCritical("Type {EndpointName} is not an EndPointBase", EndpointName);
            return null;
        }
        return endPoint;
    }

    private object? CreateEndpointServiceInstance(IServiceProvider serviceProvider)
    {
        var endpointInstance = serviceProvider.GetService(_endpointType);
        if (endpointInstance is not null)
        {
            return endpointInstance;
        }
        try
        {
            return ActivatorUtilities.CreateInstance(serviceProvider, _endpointType);
        }
        catch (Exception ex)
        {
            var message = $"Error creating instance of {EndpointName}. {ex.GetNestedMessage()}";
            Log(LogLevel.Critical, ex, "HttpRequestExecutor.ErrorCreatingInstance", message);
            return null;
        }
    }

    private async Task<ResponseResult> GetResponseAsync()
    {
        var parameterValuesResult = await GetParameterValuesAsync();
        if (parameterValuesResult.IsFailure)
        {
#if DEBUG
            Debugger.Break();
#endif
            Log(parameterValuesResult.Error, null);
            return ResponseResult.Error(parameterValuesResult.Error);
        }

        if (CancellationToken.IsCancellationRequested)
        {
            return ResponseResult.Error(
                ExtendedHttpStatusCode.RequestAborted,
                "HttpRequestExecutor.RequestCancelled",
                "Request was canceled");
        }

        var methodResult = _descriptor.Method.Invoke(
            _endpointInstance,
            parameterValuesResult.Value);

        if (methodResult is not Task task)
        {
            return GetSuccessResult(methodResult!);
        }

        await task.ConfigureAwait(false);

        var taskResult = task.GetResult();
        if (taskResult is not IResultValue resultValue)
        {
            return GetSuccessResult(taskResult!);
        }

        if (resultValue.IsSuccess)
        {
            return GetSuccessResult(resultValue.Value);
        }

        Log(resultValue.Error, null);

        return ResponseResult.Error(resultValue.Error);
    }

    private ResponseResult GetSuccessResult(object response)
    {
        var successStatusCode = _descriptor.SuccessStatusCode;
        if (successStatusCode != HttpStatusCode.OK)
        {
            return ResponseResult.SuccessWithStatus(successStatusCode, response);
        }
        return ResponseResult.Ok(response);
    }

    private async Task<Result<object?[]>> GetParameterValuesAsync()
    {
        var parameters = _descriptor.Parameters;
        if (parameters.Length == 0)
        {
            return Result.Success(Array.Empty<object?>());
        }

        var parameterValuesResult = await GetParameterValuesAsync(parameters);
        if (parameterValuesResult.IsFailure)
        {
            return Result.Failure<object?[]>(parameterValuesResult.Error);
        }

        var parameterValues = parameterValuesResult.Value!;
        if (!_descriptor.HasCancellationToken)
        {
            return Result.Success(parameterValues);
        }

        var parameterValuesWithToken = new object?[parameters.Length + 1];
        for (var i = 0; i < parameters.Length; i++)
        {
            parameterValuesWithToken[i] = parameterValues[i];
        }
        parameterValuesWithToken[^1] = CancellationToken;
        return Result.Success(parameterValuesWithToken);
    }

    private async Task<Result<object?[]>> GetParameterValuesAsync(ParameterInfo[] parameters)
    {
        if (parameters.Length == 0)
        {
            return Result.Success(Array.Empty<object?>());
        }

        if (parameters.Length == 1)
        {
            var parameter = parameters[0];
            if (parameter.ParameterType.ImplementsGenericInterfaceDefinition(typeof(IRequest<>)))
            {
                var valueResult = await BodyParameterBinder.BindParameterAsync(
                    _descriptor,
                    _httpContext,
                    parameter);

                if (valueResult.IsSuccess)
                {
                    return Result.Success(new object?[] { valueResult.Value });
                }
                return Result.Failure<object?[]>(valueResult.Error);
            }
        }

        var parameterValues = new object?[parameters.Length];
        for (var i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            var parameterValueResult = GetParameterValue(parameter);
            if (parameterValueResult.IsFailure)
            {
                return Result.Failure<object?[]>(parameterValueResult.Error);
            }
            parameterValues[i] = parameterValueResult.Value;
        }
        return Result.Success(parameterValues);
    }

    private Result<object> GetParameterValue(ParameterInfo parameter)
    {
        if (_descriptor.OperationParameterToKeyMap.ContainsKey(parameter))
        {
            var queryKey = _descriptor.OperationParameterToKeyMap[parameter];
            return OperationParameterBinder.BindParameter(
                _descriptor,
                _httpContext,
                parameter,
                queryKey);
        }

        if (_descriptor.RouteParameters.Contains(parameter))
        {
            return RouteParameterBinder.BindParameter(
                _descriptor,
                _httpContext,
                parameter);
        }

        if (parameter.HasDefaultValue)
        {
            return Result.Success(parameter.DefaultValue!);
        }

        return Result.Failure<object>(
            new BadRequestError(
                "ParameterNotFound",
                $"Error in method '{_descriptor.Method.Name}' of '{_descriptor.Method.DeclaringType?.Name}': " +
                $"Missing required parameter '{parameter.Name}'."));
    }

    private JsonSerializerOptions GetJsonSerializerOptions()
    {
        var options = _endpointInstance?.GetJsonSerializerOptions()
            ?? JsonSerializerOptions.Web;

        JsonUtils.EnableIndentationInDevelopment(options);
        return options;
    }

    private void Log(Error error, Exception? exception)
    {
        var level = ErrorLogLevelMapper.MapErrorLevel(error);
        Log(level, exception, error.Code, error.Message);
    }

    private void Log(
        LogLevel logLevel,
        Exception? exception,
        string errorCode,
        string errorMessage)
    {

        var requestUri = _httpContext.Request.GetDisplayUrl();
        var methodName = _descriptor.Method.Name;
        var httpVerb = _descriptor.HttpVerb;

#if DEBUG
        if (logLevel != LogLevel.Information)
        {
            Debugger.Break();
        }
#endif

        _logger.Log(
             logLevel,
             exception,
            "HTTP request. URI: {Uri}, Verb: {HttpVerb}, Endpoint {EndpointName}, Method: {MethodName}, ErrorCode: {ErrorCode}, ErrorMessage: {ErrorMessage}",
             requestUri,
             httpVerb,
             EndpointName,
             methodName,
             errorCode,
             errorMessage);
    }
}
