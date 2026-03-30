namespace Snebur.ClientGateway.Common.Exceptions;

public class CreateHttpRequestMessageException : Exception
{
    public CreateHttpRequestMessageException(
        string? message, 
        Exception? innerException)
        : base(message, innerException)
    {
    }
}

