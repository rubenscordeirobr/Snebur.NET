using Snebur.Domain.Primitives;

namespace Snebur.Persistence.Identity.Extensions;

internal static class EntityBaseExtensions
{
    internal static void SetCreateSession(
        this EntityBase entity,
        Guid createdSession_Id)
    {
        var nowUtc = DateTime.UtcNow;
        entity.SetPropertyValue(x => x.CreatedSession_Id, createdSession_Id);
        entity.SetPropertyValue(x => x.LastUpdatedSession_Id, createdSession_Id);
        entity.SetPropertyValue(x => x.CreatedAt, nowUtc);
        entity.SetPropertyValue(x => x.LastUpdatedAt, nowUtc);
    }
}
