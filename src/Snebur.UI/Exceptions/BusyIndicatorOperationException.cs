namespace Snebur.UI.Exceptions;

public class BusyIndicatorOperationException : Exception
{
    public BusyIndicatorOperationException(string message)
        : base(message)
    {
    }
    public BusyIndicatorOperationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

