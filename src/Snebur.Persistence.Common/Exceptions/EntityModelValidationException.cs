namespace Snebur.Persistence.Common.Exceptions;

public class EntityModelValidationException : EntityException
{
    public EntityModelValidationException(string message) : base(message)
    {
    }
}

