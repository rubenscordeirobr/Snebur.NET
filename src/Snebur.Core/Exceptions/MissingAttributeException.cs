namespace Snebur.Core.Exceptions;

public class MissingAttributeException : Exception
{
    public MissingAttributeException(string attributeName, string targetType)
        : base($"Attribute '{attributeName}' is not defined in '{targetType}'.")
    {
    }
}
