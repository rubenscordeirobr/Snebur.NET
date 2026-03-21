using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Authentication;

namespace Snebur.Core.Mappers;

public static class HttpErrorMapper
{
    public static HttpStatusCode MapErrorToHttpStatusCode(Error error)
    {
        return error switch
        {
            NoContentError => HttpStatusCode.NoContent, //204
            BadRequestError => HttpStatusCode.BadRequest, //400
            AuthenticationError => HttpStatusCode.Unauthorized, //401
            ForbiddenError => HttpStatusCode.Forbidden, //403
            NotFoundError => HttpStatusCode.NotFound, //404
            CriticalNotFoundError => HttpStatusCode.InternalServerError, //404
            DomainEventError => HttpStatusCode.Conflict, // 409
            ValidationError => HttpStatusCode.UnprocessableContent, //422
            TooManyRequestsError => HttpStatusCode.TooManyRequests, //429
            AbortedError => (HttpStatusCode)499,
            NotImplementedError => HttpStatusCode.NotImplemented, //501
            _ => HttpStatusCode.InternalServerError //500
        };
    }

    public static Error MapHttpStatusCodeToError(
        HttpStatusCode statusCode,
        string code,
        string message)
    {
        return statusCode switch
        {
            HttpStatusCode.NoContent
                => new NoContentError(code, message), //204

            HttpStatusCode.BadRequest
                => new BadRequestError(code, message), //400

            HttpStatusCode.Unauthorized
                   => new AuthenticationError(code, message), //401

            HttpStatusCode.Forbidden
                 => new ForbiddenError(code, message), //403

            HttpStatusCode.NotFound
                => new NotFoundError(code, message), //404

            HttpStatusCode.Conflict
                => new DomainEventError(code, message), //409

            HttpStatusCode.UnprocessableContent
                => new ValidationError(code, message), //422

            HttpStatusCode.TooManyRequests
                => new TooManyRequestsError(code, message), //429 

            HttpStatusCode.InternalServerError //500
                => new InternalServerError(null!, code, message),

            _ => TryMapExtendedStatus(statusCode, code, message)
        };
    }

    public static Error MapExceptionToError(
        Exception exception,
         string code)
    {
        Guard.NotNull(exception);

        var message = exception.GetNestedMessage();
        return exception switch
        {
            AuthenticationException
                => new AuthenticationError(code, message), //401

            ForbiddenSecurityException
                => new ForbiddenError(code, message), //403

            CriticalNotFoundException
                => new CriticalNotFoundError(code, message), //500

            ValidationException
                => new ValidationError(code, message), //422
            TaskCanceledException
                => new AbortedError(code, message), //499

            OperationCanceledException operationCanceledException
                => new OperationCanceledError(operationCanceledException, code, message),

            _ => new InternalServerError(exception, code, message)
        };
    }

    private static Error TryMapExtendedStatus(
        HttpStatusCode statusCode,
        string code,
        string message)
    {
        var extendStatus = (ExtendedHttpStatusCode)statusCode;
        if (Enum.IsDefined(extendStatus))
        {
            return extendStatus switch
            {
                ExtendedHttpStatusCode.RequestAborted => new AbortedError(code, message),
                _ => new NotImplementedError($"ExtendedHttpStatusCode.NotImplemented.{code}", message)
            };
        }

        return new NotImplementedError($"HttpStatusCode.NotImplemented.{code}", message);

    }
}

