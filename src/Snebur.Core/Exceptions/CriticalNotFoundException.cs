namespace Snebur.Core.Exceptions;

public class CriticalNotFoundException : Exception
{
    public CriticalNotFoundException(string message)
        : base(message)
    {
    }
}

