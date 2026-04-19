using Snebur.ClientGateway.Common.Abstractions;
using Snebur.UI.Components.Dialogs;

namespace Snebur.UI.Services;

public class ConnectionStatusNotifier : IConnectionStatusNotifier
{
    private readonly IJsonStringLocalizer<ConnectionStatusNotifier> _localizer;
    private readonly IDialogService _dialogService;

    private IDialogReference? _currentDialog;

    public ConnectionStatusNotifier(
        IJsonStringLocalizer<ConnectionStatusNotifier> localizer,
        IDialogService dialogService)
    {
        _localizer = localizer;
        _dialogService = dialogService;

    }

    public async Task NotifyConnectionLostAsync()
    {
        if (_currentDialog is not null)
            return;

        var dialogParameters = new DialogParameters
        {
            Width = "30rem",
            Height = "15rem",
            PreventDismissOnOverlayClick = true,
            DialogBodyStyle = "z-index: 99999",
            ShowTitle = false,
            ShowDismiss  = false,
            PrimaryAction = null,
            SecondaryAction = null,
            PrimaryActionEnabled = false,
            SecondaryActionEnabled = false,
        };

        _currentDialog = await _dialogService.ShowDialogAsync<ConnectionStatusDialog>(dialogParameters);
    }

    public async Task NotifyConnectionRestoredAsync()
    {
        if (_currentDialog is null)
            return;

        await _currentDialog.CloseAsync();
        _currentDialog = null;
    }
}
