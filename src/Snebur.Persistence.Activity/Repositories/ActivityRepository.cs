using System.Linq.Expressions;
using Snebur.Application.Abstractions.Persistence.Activities;
using Snebur.Core;
using Snebur.Domain.Entities.Activities;
using Snebur.Persistence.Activity.Documents;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Snebur.Persistence.Activity.Repositories;

public class ActivityRepository : IActivityRepository
{
    private const int ACTIVITY_QUERY_LIMIT = 100;
    private readonly IMongoCollection<ActivityDocument> _collection;
    private readonly ILogger _logger;

    public ActivityRepository(
        IMongoDatabase database,
        ILogger<ActivityRepository> logger)
    {
        Guard.NotNull(database);

        _collection = database.GetCollection<ActivityDocument>("activities");
        _logger = logger;
    }

    public async Task<IEnumerable<ActivityBase>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await QueryAsync(_ => true, cancellationToken: cancellationToken);
    }

    private async Task<IEnumerable<ActivityBase>> QueryAsync(
        Expression<Func<ActivityDocument, bool>> filter,
        FindOptions? options = null,
        int limit = ACTIVITY_QUERY_LIMIT,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var documents = await _collection
                .Find(filter, options)
                .SortByDescending(d => d.ActivityAt)
                .Limit(limit)
                .ToListAsync(cancellationToken);

            return documents.Select(ActivityDocumentMapper.MapToDomain);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error querying activities");
            throw;
        }
    }

    public async Task<ActivityBase?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var result = await QueryAsync(d => d.Id == id, limit: 1, cancellationToken: cancellationToken);
        return result.FirstOrDefault();
    }

    public async Task AddAsync(ActivityBase activity, CancellationToken cancellationToken = default)
    {
        var document = ActivityDocumentMapper.MapToDocument(activity);
        try
        {
            await _collection.InsertOneAsync(document, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding activity");
            throw;
        }
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            await _collection.DeleteOneAsync(d => d.Id == id, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting activity");
            throw;
        }
    }
}
