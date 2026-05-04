
namespace Snebur.ClientGateway.Abstractions;

public interface IClientUserSessionContextService
{
    Task InitializeAsync();
 
    Task ClearSessionContextAsync();
}
