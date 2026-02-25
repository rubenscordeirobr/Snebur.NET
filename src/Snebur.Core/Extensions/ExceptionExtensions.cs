namespace Snebur.Core.Extensions;

public static class ExceptionExtensions
{
    public static string GetNestedMessage(this Exception exception)
    {
        Guard.NotNull(exception);

        var messages = new List<string>();
        var current = exception;
        while (current != null)
        {
            messages.Add(current.Message);
            current = current.InnerException;
        }
        return string.Join(" -> ", messages);
    }
}
