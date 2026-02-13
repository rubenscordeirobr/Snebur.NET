namespace Snebur.UseCases.Common;

public abstract record GetEntityByIdQuery<TResponse>(Guid Id) : QueryRequest<TResponse>
    where TResponse : IResponse;
