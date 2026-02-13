namespace Snebur.Application.Exceptions;

public class HandlerRegistrationAlreadyExistsException :Exception
{
    public HandlerRegistrationAlreadyExistsException(string message) : base(message)
    {
    }
}
