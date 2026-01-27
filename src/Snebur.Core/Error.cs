using System.Diagnostics;
using System.Net;
using Snebur.Core.Mappers;

namespace Snebur.Core;

[DebuggerDisplay("Type = {GetType().Name}, Code = {Code}, Message = {Message}")]
public abstract record Error
{
    public string Code { get; }
    public string Message { get; }
    public Exception? Exception { get; }

    protected Error(string code, string message)
    {
        Guard.NotNullOrWhiteSpace(code);
        Guard.NotNullOrWhiteSpace(message);

        Code = code;
        Message = message;
    }
    protected Error(Exception? exception, string code, string message)
    {
        Guard.NotNullOrWhiteSpace(code);
        Guard.NotNullOrWhiteSpace(message);

        Code = code;
        Message = message;
        Exception = exception;
    }

    public virtual HttpStatusCode StatusCode
        => HttpErrorMapper.MapErrorToHttpStatusCode(this);

    public ErrorResponse CreateErrorResponse()
     => new ErrorResponse(Code, Message);

    public sealed override string ToString()
        => $"{Code}: {Message}";

    public static implicit operator string(Error error)
        => error?.ToString() ?? string.Empty;

}

public record BadRequestError(string Code,
    string Message)
    : Error(Code, Message);

public record ValidationError(
    string Code,
    string Message)
    : Error(Code, Message);

public record CommandValidatorNotFoundError(
    Exception Exception,
    string Code,
    string Message)
    : Error(Exception, Code, Message);

public record NotFoundError(
    string code,
    string message)
    : Error(code, message);

public record CriticalNotFoundError(
    string code,
    string message)
    : Error(code, message);

public record InvalidOperationError(
    string Code,
    string Message)
    : Error(Code, Message);

public record ForbiddenError(
    string Code,
    string Message)
    : Error(Code, Message);

public record DomainEventError(
    string Code,
    string Message)
    : Error(Code, Message);

public record InternalServerError(
    Exception? Exception,
    string Code,
    string Message)
    : Error(Exception, Code, Message);

public record DatabaseError(
    Exception Exception,
    string Code,
    string Message)
    : Error(Exception, Code, Message);

public record OperationCanceledError(
    Exception? Exception,
    string Code,
    string Message)
    : Error(Exception, Code, Message);

public record UnknownError(
    Exception? Exception,
    string Code,
    string Message)
    : Error(Exception, Code, Message);

public partial record DeserializationError(
    Exception Exception,
    string Code,
    string Message)
    : Error(Exception, Code, Message);

public record NotImplementedError(
    string Code,
    string Message)
    : Error(Code, Message);

public record AbortedError(
    string Code,
    string Message)
    : Error(Code, Message);

// Used specifically for serialization to API responses
public record ErrorResponse(
    string Code,
    string Message);

public partial record DeserializationError
{
    public static DeserializationError Create<T>(
        Exception ex,
        string code,
        string json)
    {

        json = json.SafeTrim(1024, "[truncated]");
        return new DeserializationError(ex,
               code,
               $"An error occurred while deserializing. " +
               $"Type : {typeof(T).Name}" +
               $"Message: {ex?.Message}\r\n" +
               $"Json: {json}");
    }
}
public record CreateHttpRequestMessageError(
    Exception Exception,
    string Code,
    string Message)
    : Error(Exception, Code, Message);

public record RequestError(
    Exception Exception,
    string Code,
    string Message)
    : Error(Exception, Code, Message);

public record TaskTimeoutError(
    Exception? Exception,
    string Code,
    string Message)
    : Error(Exception, Code, Message);

public record TooManyRequestsError(
    string Code,
    string Message)
    : Error(Code, Message);

public record AuthenticationError(
    string Code,
    string Message)
    : Error(Code, Message);

public record ParserError(
    Exception? Exception,
    string Code,
    string Message)
    : Error(Exception, Code, Message);

public record NoContentError(
    string Code,
    string Message)
    : Error(Code,
            Message + " For responses with no content, please use OperationResponse.");

public record AzureServiceError(
     Exception? Exception,
     string Code,
     string Message)
    : Error(Exception, Code, Message);

public record TranslationServiceError(
     Exception? Exception,
     string Code,
     string Message)
    : Error(Exception, Code, Message);

public record OperationInProgressError(
    string Code,
    string Message)
    : Error(Code, Message);
