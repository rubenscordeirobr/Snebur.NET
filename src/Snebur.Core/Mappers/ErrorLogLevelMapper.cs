using Microsoft.Extensions.Logging;

namespace Snebur.Core.Mappers;

public static class ErrorLogLevelMapper
{
    public static LogLevel MapErrorLevel(Error error)
    {
        return error switch
        {
            NotFoundError => LogLevel.Information,
            OperationCanceledError => LogLevel.Information,

            // Warnings
            ValidationError => LogLevel.Warning,
            ForbiddenError => LogLevel.Warning,
            AbortedError => LogLevel.Warning,
            TaskTimeoutError => LogLevel.Warning,

            // Errors
            NoContentError => LogLevel.Error,
            BadRequestError => LogLevel.Error,
            DomainEventError => LogLevel.Error,
            RequestError => LogLevel.Error,
            DeserializationError => LogLevel.Error,
            CreateHttpRequestMessageError => LogLevel.Error,
            UnknownError => LogLevel.Error,
            InvalidOperationError => LogLevel.Error,
            NotImplementedError => LogLevel.Critical,

            //Critical
            CriticalNotFoundError => LogLevel.Critical,
            DatabaseError => LogLevel.Critical,
            CommandValidatorNotFoundError => LogLevel.Critical,
            InternalServerError => LogLevel.Critical,
            _ => LogLevel.Error
        };
    }
}
