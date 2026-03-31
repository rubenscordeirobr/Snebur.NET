
namespace Snebur.Domain.Entities.Activities;

public sealed record UserLogoutActivity : ActivityBase
{
    public required Guid User_Id { get; init; }
    public required string IpAddress { get; init; }
    public override ActivityType ActivityType
        => ActivityType.UserLogout;
}
