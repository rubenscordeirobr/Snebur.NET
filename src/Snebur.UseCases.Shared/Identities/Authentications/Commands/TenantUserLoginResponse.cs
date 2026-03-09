namespace Snebur.UseCases.Identities.Authentications.Commands;

public record TenantUserLoginResponse : IResponse
{
    public required string AuthorizationToken { get; set; }
    public required UserSessionResponse UserSession { get; init; }
    public required UserResponse User { get; init; }
    public required TenantResponse Tenant { get; init; }
}

