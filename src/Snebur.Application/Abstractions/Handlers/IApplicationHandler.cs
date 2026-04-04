namespace Snebur.Application.Abstractions.Handlers;
public interface IApplicationHandler
{
    Task HandleAsync(object handlerObject, CancellationToken cancellationToken = default);
}
