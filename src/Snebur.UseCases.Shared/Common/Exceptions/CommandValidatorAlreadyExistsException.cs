namespace Snebur.UseCases.Common.Excpetions;

public class CommandValidatorAlreadyExistsException : Exception
{
    public CommandValidatorAlreadyExistsException(string message) : base(message)
    {
    }
}
