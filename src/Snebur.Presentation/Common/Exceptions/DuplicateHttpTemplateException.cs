namespace Snebur.Presentation.Common.Exceptions;

public class DuplicateHttpTemplateException : HttpTemplateException
{
    public DuplicateHttpTemplateException(string message) : base(message)
    {
    }
}
