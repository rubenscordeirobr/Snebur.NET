using Snebur.Domain.Entities.Activities;

namespace Snebur.Testing.Core.Mocks.Repositories;

public class ActivityRepositoryMock : IActivityRepository
{
    public Task<IEnumerable<ActivityBase>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<ActivityBase>>([]);
    }

    public Task<ActivityBase?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<ActivityBase?>(null);
    }

    public Task AddAsync(ActivityBase activity, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
