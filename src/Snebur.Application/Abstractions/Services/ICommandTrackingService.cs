namespace Snebur.Application.Abstractions.Services;

public interface ICommandTrackingService : IApplicationService
{
    Task<bool> ExistsAsync(
        Guid clientRequestId,
        CancellationToken cancellationToken = default);

    Task<Result<T>?> TryGetResultAsync<T>(
        Guid clientRequestId,
        CancellationToken cancellationToken = default)
        where T : notnull;

    Task TrackAsync<T>(Guid clientRequestId, Result<T> result) where T : notnull;
}
