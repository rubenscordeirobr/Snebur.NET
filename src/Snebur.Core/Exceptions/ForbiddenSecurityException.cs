namespace Snebur.Core.Exceptions;

public class ForbiddenSecurityException : Exception
{
    public ForbiddenSecurityException(string message) 
        : base(message)
    {
    }
}

