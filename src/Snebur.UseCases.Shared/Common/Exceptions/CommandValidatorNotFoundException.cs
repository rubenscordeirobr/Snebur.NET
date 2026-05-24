namespace Snebur.UseCases.Common.Excpetions;

public class CommandValidatorNotFoundException : Exception
{
    public IReadOnlyList<Type> CommandTypes { get; }
 
    public CommandValidatorNotFoundException(string message,
        IReadOnlyList<Type> commandTypes)
        : base(message)
    {
        CommandTypes = commandTypes;
    }
}
