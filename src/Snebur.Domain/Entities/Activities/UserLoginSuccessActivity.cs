
namespace Snebur.Domain.Entities.Activities;

public sealed record UserLoginSuccessActivity : ActivityBase
{
    public required Guid User_Id { get; init; }
    public required AuthenticationType AuthenticationType { get; init; }
    public required string IpAddress { get; init; }
    public required string UserIdentifier  { get; init; }
    public override ActivityType ActivityType
        => ActivityType.UserLoginSuccess;
}
