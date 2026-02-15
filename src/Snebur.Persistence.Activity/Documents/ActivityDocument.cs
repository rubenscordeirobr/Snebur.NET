using Snebur.SharedKernel.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Snebur.Persistence.Activity.Documents;

public class ActivityDocument 
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    public ActivityType ActivityType { get; set; }

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime ActivityAt { get; set; } = DateTime.UtcNow;
 
    [BsonRepresentation(BsonType.String)]
    public Guid UserSession_Id { get; set; }
 
    [BsonRepresentation(BsonType.String)]
    public Guid? Tenant_Id { get; set; }
  
    [BsonRepresentation(BsonType.String)]
    public Guid? Entity_Id { get; set; }

    public AuthenticationType? AuthenticationType { get; set; }

    public string? QualifiedTypeName { get; set; }
    public string? Description { get; set; }
    public string? CreatedData { get; set; }
    public string? OldData { get; set; }
    public string? NewData { get; set; }
    public string? DeletedData { get; set; }

 
    public string? UserIdentifier { get; set; }
    public string? IpAddress { get; set; }
    public string? PasswordFailed { get; set; }
}
