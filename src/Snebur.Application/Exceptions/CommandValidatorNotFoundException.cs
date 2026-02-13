namespace Snebur.Application.Exceptions;

public class CommandValidatorNotFoundException : Exception
{
    public CommandValidatorNotFoundException(
        string message,
        Exception? innerException = null) : base(message, innerException)
    {
    }
}
