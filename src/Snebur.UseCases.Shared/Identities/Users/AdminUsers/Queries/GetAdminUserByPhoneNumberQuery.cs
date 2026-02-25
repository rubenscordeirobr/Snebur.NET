namespace Snebur.UseCases.Identities.Users.AdminUsers.Queries;

public record GetAdminUserByPhoneNumberQuery(
    string PhoneNumber)
    : QueryRequest<UserResponse>;
