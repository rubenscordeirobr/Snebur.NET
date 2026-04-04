using Snebur.Domain.Entities.Activities;

namespace Snebur.Application.Abstractions.Persistence.Activities;
public interface IActivityRepository
{
    Task<IEnumerable<ActivityBase>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ActivityBase?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task AddAsync(ActivityBase activity, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}
