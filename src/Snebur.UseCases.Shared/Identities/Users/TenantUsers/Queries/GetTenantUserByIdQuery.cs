namespace Snebur.UseCases.Identities.Users.TenantUsers.Queries;

public record GetTenantUserByIdQuery(Guid Id) 
    : GetEntityByIdQuery<UserResponse>(Id);
