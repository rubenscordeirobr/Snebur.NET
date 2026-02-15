namespace Snebur.Persistence.Common.Exceptions;

public abstract class EntityException : Exception
{
    protected EntityException(string message) 
        : base(message)
    {
    }

    protected EntityException(string message, Exception? innerException) 
        : base(message, innerException)
    {
    }
}

