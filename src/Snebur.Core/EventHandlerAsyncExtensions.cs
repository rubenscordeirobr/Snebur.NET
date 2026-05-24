using Microsoft.Extensions.Logging;

namespace Snebur.Core;

public static class EventHandlerAsyncExtensions
{
    public static Task InvokeAsync(
        this EventHandlerAsync? handler,
        object? sender, EventArgs args)
    {
        return InvokeAsyncInternal(handler, sender, args);
    }

    public static Task InvokeAsync<TEventArgs>(
        this EventHandlerAsync<TEventArgs>? handler,
        object? sender, TEventArgs args)
        where TEventArgs : EventArgs
    {
        return InvokeAsyncInternal(handler, sender, args);
    }

    public static Task<EventHandlerAsyncResult> TryInvokeAsync(
        this EventHandlerAsync? handler,
        object? sender, EventArgs args,
        ILogger? logger = null)
    {
        return TryInvokeAsyncInternal(handler, sender, args, logger);
    }

    public static Task<EventHandlerAsyncResult> TryInvokeAsync<TEventArgs>(
       this EventHandlerAsync<TEventArgs>? handler,
       object? sender, TEventArgs args,
       ILogger? logger = null)
       where TEventArgs : EventArgs
    {
        return TryInvokeAsyncInternal(handler, sender, args, logger);
    }

    private static async Task InvokeAsyncInternal<TDelegate, TEventArgs>(
        TDelegate? handler,
        object? sender, TEventArgs args)
        where TDelegate : Delegate
        where TEventArgs : EventArgs
    {
        var invocationList = handler?.GetInvocationList()?.Cast<EventHandlerAsync<TEventArgs>>().ToList();
        if (invocationList == null)
        {
            return;
        }
        foreach (var asyncHandler in invocationList)
        {
            await asyncHandler(sender, args);
        }
    }

    private static async Task<EventHandlerAsyncResult> TryInvokeAsyncInternal<TDelegate, TEventArgs>(
        TDelegate? handler,
        object? sender, TEventArgs args,
        ILogger? logger = null)
        where TDelegate : Delegate
        where TEventArgs : EventArgs
    {
        var invocationList = handler?.GetInvocationList()?.Cast<EventHandlerAsync<TEventArgs>>().ToList();
        if (invocationList == null)
        {
            return new EventHandlerAsyncResult(Enumerable.Empty<Exception>());
        }

        var errors = new List<Exception>();
        foreach (var asyncHandler in invocationList)
        {
            try
            {
                await asyncHandler(sender, args);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error invoking event handler: {Handler}", asyncHandler.Method.Name);
                errors.Add(ex);
            }
        }
        return new EventHandlerAsyncResult(errors);
    }
}
