namespace Snebur.UseCases.Identities.Users.AdminUsers.Queries;

public record GetAdminUserByIdQuery(Guid Id) 
    : GetEntityByIdQuery<UserResponse>(Id);
