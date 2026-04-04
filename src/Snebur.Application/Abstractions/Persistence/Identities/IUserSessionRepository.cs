
namespace Snebur.Application.Abstractions.Persistence.Identities;

public interface IUserSessionRepository : IRepositoryBase<UserSession>
{
    Task<UserSession?> GetByIdWithUserAsync(
        Guid session_Id,
        CancellationToken cancellationToken = default);
}
