namespace Snebur.Persistence.Common.Exceptions;

internal class TransactionOpenException : EntityException
{
    public TransactionOpenException(
        string message,
        Exception? innerException) : base(message, innerException)
    {
    }
}
