namespace Snebur.UseCases.Identities.Authentications.Commands;
public class AdminUserLoginResponse : IResponse
{
    public required string AuthorizationToken { get; set; }
    public required UserSessionResponse UserSession { get; init; }
    public UserResponse User { get; init; }
}
