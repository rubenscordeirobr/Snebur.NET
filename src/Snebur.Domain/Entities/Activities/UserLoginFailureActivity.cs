namespace Snebur.Domain.Entities.Activities;

public sealed record UserLoginFailureActivity : ActivityBase
{
    public required Guid User_Id { get; init; }
    public required AuthenticationType AuthenticationType { get; init; }
    public required string IpAddress { get; init; }
    public required string UserIdentifier { get; init; }
    public required string PasswordFailed { get; init; }

    public sealed override ActivityType ActivityType
        => ActivityType.UserLoginFailed;

}
