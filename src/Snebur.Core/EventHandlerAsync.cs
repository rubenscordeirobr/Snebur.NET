namespace Snebur.Core;

public delegate Task EventHandlerAsync(object? sender, EventArgs e);

public delegate Task EventHandlerAsync<in TEventArgs>(object? sender, TEventArgs e) 
    where TEventArgs : EventArgs;

public class EventHandlerAsyncResult
{
    public IReadOnlyList<Exception> Errors { get; }
    public bool HasErrors => Errors.Count > 0;

    public bool Success => !HasErrors;
    public void ThrowIfHasErrors()
    {
        if (HasErrors)
        {
            throw new AggregateException(Errors);
        }
    }

    public EventHandlerAsyncResult(IEnumerable<Exception> errors)
    {
        Guard.NotNull(errors);
        Errors = errors.ToArray();
    }
}
