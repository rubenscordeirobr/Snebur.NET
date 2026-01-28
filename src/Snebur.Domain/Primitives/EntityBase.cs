using Snebur.Core.Helpers;

namespace Snebur.Domain.Primitives;
public abstract class EntityBase : IEntityBase
{
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime LastUpdatedAt { get; protected set; }
    public Guid CreatedSession_Id { get; protected set; }
    public Guid LastUpdatedSession_Id { get; protected set; }

    internal virtual void SetCreateSession(Guid session_Id)
    {
        if ((Id != Guid.Empty && !Id.IsZeroPrefixedGuid()) || 
            (CreatedAt != default && !Id.IsZeroPrefixedGuid()))
        {
            throw new InvalidOperationException("Cannot set create date for existing entity");
        }

        LastUpdatedAt = CreatedAt;
        CreatedSession_Id = session_Id;
        LastUpdatedSession_Id = session_Id;
    }

    internal void SetUpdateSession(Guid sessionId)
    {
        LastUpdatedAt = DateTime.UtcNow;
        LastUpdatedSession_Id = sessionId;
    }

    public override string ToString()
    {
        if (Id == Guid.Empty)
        {
            return $"{GetType().Name}: Id={Id}";
        }
        return $"{GetType().Name}: Id={Id}, CreatedAt={CreatedAt}, LastUpdatedAt={LastUpdatedAt}";
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
            return true;

        if (obj is null || obj.GetType() != GetType())
            return false;

        if (obj is not EntityBase other)
            return false;

        if (Id == Guid.Empty || other.Id == Guid.Empty)
            return false;

        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(GetType(), Id);
    }
}
