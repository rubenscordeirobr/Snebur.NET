using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Snebur.UI.Core;

public abstract class ViewModelBase : IDisposable, INotifyPropertyChanged
{
    private bool _isBusy;
    private readonly IBusyIndicatorService _busyIndicatorService;
    private readonly ILogger _logger;
    private readonly SemaphoreSlim _busySemaphore = new(1, 1);

    public EditContext EditContext { get; }
    public ValidationMessageStore MessageStore { get; }

    public bool IsBusy
    {
        get => _isBusy;
        private set => SetProperty(ref _isBusy, value);
    }

    protected ViewModelBase(
        IBusyIndicatorService busyIndicatorService,
        ILogger logger)
    {
        _busyIndicatorService = busyIndicatorService;
        _logger = logger;

        EditContext = new EditContext(this);
        MessageStore = new ValidationMessageStore(EditContext);
        EditContext.OnFieldChanged += EditContext_OnFieldChanged;
    }

    public void AddError(string errorMessage, string propertyName = "")
    {
        MessageStore.Clear();
        MessageStore.Add(EditContext.Field(propertyName), errorMessage);
        EditContext.NotifyValidationStateChanged();
    }

    protected async Task<Result<T>> RunWithBusyIndicatorAsync<T>(
       Task<Result<T>> operation)
       where T : notnull
    {
        Guard.NotNull(operation);
        return await RunWithBusyIndicatorAsync(() => operation);
    }

    protected async Task<Result<T>> RunWithBusyIndicatorAsync<T>(
        Func<Task<Result<T>>> operation,
        CancellationToken cancellationToken = default)
        where T : notnull
    {
        Guard.NotNull(operation);

        if (IsBusy || !await _busySemaphore.WaitAsync(0, cancellationToken))
        {
            return Result.Failure<T>(CreateInProgressError());
        }

        try
        {
            IsBusy = true;
            _busyIndicatorService.Busy();

            var result = await operation();
            if (result.IsFailure)
            {
                AddError(result.Error.Message);
            }
            return result;
        }
        catch (Exception ex)
        {
            AddError(ex.Message);

            var errorCode = this.GetErrorCode(nameof(RunWithBusyIndicatorAsync));
            var error = new UnknownError(ex, errorCode, ex.Message);
            _logger.LogError(ex, "An error occurred while executing the operation. Error code: {ErrorCode}. Message: {Message}",
                    errorCode,
                    ex.GetNestedMessage());

            return Result.Failure<T>(error);
        }
        finally
        {
            _busySemaphore.Release();
            _busyIndicatorService.Release();
            IsBusy = false;
        }
    } 

    private void EditContext_OnFieldChanged(object? sender, FieldChangedEventArgs e)
    {
        MessageStore.Clear(EditContext.Field(""));
        EditContext.Validate();
    }

    private OperationInProgressError CreateInProgressError()
    {
        var code = this.GetErrorCode(nameof(RunWithBusyIndicatorAsync));
        _logger.LogError("Operation already in progress. Code: {ErrorCode}", code);
        return new OperationInProgressError(code, "Operation is already in progress.");
    }

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string? propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        EditContext.NotifyValidationStateChanged();
    }

    protected bool SetProperty<T>(
        ref T field, T value, 
        [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;
      
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
    #endregion

    #region IDisposable

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            EditContext.OnFieldChanged -= EditContext_OnFieldChanged;
            _busySemaphore?.Dispose();
        }
    }
    #endregion
}

