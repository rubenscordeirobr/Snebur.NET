namespace Snebur.UseCases.Common;

public abstract record class CommandRequest<TResponse>
    : ICommandRequest<TResponse>
    where TResponse : IResponse
{
    public Guid ClientRequestId { get; init; } = Guid.NewGuid();

    public DateTime? ValidatedSuccessfullyAt { get; set; }
}
