namespace Snebur.Application.Exceptions;

public class SyncSaveChangesNotAllowedException : Exception
{
    public SyncSaveChangesNotAllowedException(
        string message)
        : base(message)
    {
    }
}
