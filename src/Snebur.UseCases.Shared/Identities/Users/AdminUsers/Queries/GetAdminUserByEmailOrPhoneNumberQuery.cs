namespace Snebur.UseCases.Identities.Users.AdminUsers.Queries;

public record GetAdminUserByEmailOrPhoneNumberQuery(
    string EmailOrPhoneNumber )
    : QueryRequest<UserResponse>;
