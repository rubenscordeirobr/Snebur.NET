namespace Snebur.SharedKernel.Abstractions;

public interface IAggregateRoot
{

}

public interface IEventAggregate : IAggregateRoot
{
    public IReadOnlyList<IDomainEvent> DomainEvents { get; }
}
