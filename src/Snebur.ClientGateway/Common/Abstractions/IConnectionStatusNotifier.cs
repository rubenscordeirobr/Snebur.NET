namespace Snebur.ClientGateway.Common.Abstractions;

public interface IConnectionStatusNotifier
{
    Task NotifyConnectionLostAsync();
    Task NotifyConnectionRestoredAsync();
}
