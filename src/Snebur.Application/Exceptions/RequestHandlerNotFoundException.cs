namespace Snebur.Application.Exceptions;

public class RequestHandlerNotFoundException : Exception
{
    public RequestHandlerNotFoundException( 
        string message)
        : base(message)
    {
    }
}
