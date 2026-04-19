using System.Net;
using Snebur.ClientGateway.Common.Abstractions;
using Microsoft.Extensions.Logging;

namespace Snebur.UI.Services;

public class RequestErrorNotifier : IRequestErrorNotifier
{
    private readonly ILogger<RequestErrorNotifier> _logger;
    private readonly IDialogService _dialogService;
    public RequestErrorNotifier(
        IDialogService dialogService,
        ILogger<RequestErrorNotifier> logger)
    {
        _dialogService = dialogService;
        _logger = logger;
    }

    public async Task NotifyRequestErrorAsync(Error error, Uri requestUri)
    {
        if (IsShowErrorDialog(error?.StatusCode))
        {
            var errorMessage = error?.Message ?? "Unknown error";
            try
            {
                var dialog = await _dialogService.ShowErrorAsync(errorMessage, "Oops", "OK");
                await dialog.Result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error showing error dialog. {ErrorMessage}. Error: {Error}",
                    errorMessage,
                    ex.GetNestedMessage());
            }
        }
    }

    public static bool IsShowErrorDialog(HttpStatusCode? httpStatusCode)
    {
        if (httpStatusCode is null)
            return true;

        return httpStatusCode switch
        {
            HttpStatusCode.NotFound => false,
            HttpStatusCode.Unauthorized => false,
            _ => true
        };

    }
}
