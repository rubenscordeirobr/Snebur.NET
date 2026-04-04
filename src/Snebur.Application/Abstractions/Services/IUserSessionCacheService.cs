using Snebur.SharedKernel.Models.Identities;

namespace Snebur.Application.Abstractions.Services;

public interface IUserSessionCacheService : IApplicationService
{
    Task<bool> ExistsAsync(Guid session_Id, CancellationToken cancellationToken = default);

    Task<CachedUserSession?> GetSessionAsync(
        Guid session_Id,
        CancellationToken cancellationToken = default);

    Task AddSessionAsync(
        IUserSession session,
        CancellationToken cancellationToken = default);

    Task RemoveSessionAsync(
        Guid session_Id,
        CancellationToken cancellationToken = default);
}
