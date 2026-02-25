namespace Snebur.UseCases.Identities.Users.TenantUsers.Queries;

public record GetTenantUserByEmailQuery(
    string Email) 
    : QueryRequest<UserResponse>;
