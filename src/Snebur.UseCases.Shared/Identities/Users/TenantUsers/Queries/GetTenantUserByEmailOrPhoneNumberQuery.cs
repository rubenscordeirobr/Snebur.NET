namespace Snebur.UseCases.Identities.Users.TenantUsers.Queries;

public record GetTenantUserByEmailOrPhoneNumberQuery(
    string EmailOrPhoneNumber) 
    : QueryRequest<UserResponse>;
