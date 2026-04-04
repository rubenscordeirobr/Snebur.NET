namespace Snebur.SharedKernel.Abstractions;

public interface IRequest<TResponse>
    where TResponse : IResponse
{
}

public interface IQueryRequest
{
}

public interface IQueryRequest<TResponse> : IRequest<TResponse>, IQueryRequest
    where TResponse : IResponse
{
}

public interface ICommandRequest
{
    Guid ClientRequestId { get; }
    DateTime? ValidatedSuccessfullyAt { get; set; }
}

public interface ICommandRequest<TResponse> : IRequest<TResponse>, ICommandRequest
    where TResponse : IResponse
{
    
}
