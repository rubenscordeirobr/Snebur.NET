using System.Diagnostics;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Snebur.Presentation.Common;

internal class HttpRequestExecutorFallback
{
    private readonly HttpContext _httpContext;
    private readonly ILogger _logger;

    public HttpRequestExecutorFallback(
        HttpContext httpContext)
    {
        _httpContext = httpContext;
        _logger = httpContext.RequestServices.GetRequiredService<ILogger<HttpRequestExecutor>>();
    }

    public async Task ProcessRequestAsync()
    {
        var route = _httpContext.Request.GetPathAndQueryString();
        _logger.LogWarning(
            " Route not found {Route}", route);

        _httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

#if DEBUG
        Debugger.Break();
#endif
        var responseError= new ErrorResponse($"HttpRequestExecutorFallback.RouteNotFound", route);
        await WriteResponseAsync(responseError, _httpContext.RequestAborted);

    }
    protected async Task WriteResponseAsync(
         ErrorResponse response,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        try
        {
            var jsonOptions = JsonSerializerOptions.Web;
            await _httpContext.Response.WriteAsJsonAsync(response, jsonOptions, cancellationToken);
        }
        catch
        {
            //Do nothing
        }

    }

}

