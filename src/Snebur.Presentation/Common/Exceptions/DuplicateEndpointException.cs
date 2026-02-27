namespace Snebur.Presentation.Common.Exceptions;

public class DuplicateEndpointException: HttpTemplateException
{
    public DuplicateEndpointException(string message) : base(message)
    {
    }
}
