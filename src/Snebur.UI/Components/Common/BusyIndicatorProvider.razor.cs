using Snebur.UI.Services;

namespace Snebur.UI.Components.Common;

public partial class BusyIndicatorProvider  
{
    private long _busyCount;
    public bool IsBusy 
        => Interlocked.Read(ref _busyCount) > 0;

    [Inject]
    private IBusyIndicatorService BusyIndicatorService { get; set; } = default!;
 
    protected override void OnInitialized()
    {
        BusyIndicatorService.Initialize(BusyAsync, ReleaseAsync);
        base.OnInitialized();
    }

   
    private Task BusyAsync()
    {
        Interlocked.Increment(ref _busyCount);
        StateHasChanged();
        return Task.CompletedTask;
    }

    private async Task ReleaseAsync()
    {
        Interlocked.Decrement(ref _busyCount);
        if(_busyCount<0)
        {
            throw new InvalidOperationException(
                "Busy count cannot be negative. \r\n" +
                "Make sure to call ReleaseAsync() only when BusyAsync() has been called.");
        }
        StateHasChanged();
        await Task.CompletedTask;
    }

}

