using System.Diagnostics.CodeAnalysis;
using System.Net;
using Snebur.Core.Enums;

namespace Snebur.Presentation.Common;

internal sealed record ResponseResult
{
    [MemberNotNullWhen(false, nameof(ErrorResponse))]
    public bool IsSuccess { get; }
    public int StatusCode { get; }
    public object? Value { get; }

    public ErrorResponse? ErrorResponse { get; }

    private ResponseResult(HttpStatusCode statusCode, object value)
    {
        Value = value;
        StatusCode = (int)statusCode;
        IsSuccess = true;
    }

    private ResponseResult(HttpStatusCode statusCode, string errorCode, string errorMessage)
    {
        ErrorResponse = new ErrorResponse(errorCode, errorMessage);
        StatusCode = (int)statusCode;
        IsSuccess = false;
    }

    private ResponseResult(HttpStatusCode statusCode, ErrorResponse errorResponse)
    {
        ErrorResponse = errorResponse;
        StatusCode = (int)statusCode;
        IsSuccess = false;
    }

    private ResponseResult(ExtendedHttpStatusCode statusCode, string errorCode, string errorMessage)
    {
        ErrorResponse = new ErrorResponse(errorCode, errorMessage);
        StatusCode = (int)statusCode;
        IsSuccess = false;
    }
    public static ResponseResult Ok(object response)
        => new ResponseResult(HttpStatusCode.OK, response);

    public static ResponseResult SuccessWithStatus(
        HttpStatusCode statusCode,
        object response)
        => new ResponseResult(statusCode, response);

    public static ResponseResult Error(
        HttpStatusCode statusCode,
        string errorCode,
        string errorMessage)
        => new ResponseResult(statusCode, errorCode, errorMessage);

    public static ResponseResult Error(
        ExtendedHttpStatusCode statusCode,
        string errorCode,
        string errorMessage)
        => new ResponseResult(statusCode, errorCode, errorMessage);

    public static ResponseResult Error(Error error)
        => new ResponseResult(error.StatusCode, error.CreateErrorResponse());
}