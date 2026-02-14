namespace Snebur.Application.Exceptions;

public class DomainEventContextCancelledException : Exception
{
    public DomainEventContextCancelledException()
        : base("Domain event context was cancelled.")
    {
    }
}

