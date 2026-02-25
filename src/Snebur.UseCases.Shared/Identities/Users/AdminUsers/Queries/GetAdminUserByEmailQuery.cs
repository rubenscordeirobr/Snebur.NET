namespace Snebur.UseCases.Identities.Users.AdminUsers.Queries;

public record GetAdminUserByEmailQuery(
    string Email)
    : QueryRequest<UserResponse>;
