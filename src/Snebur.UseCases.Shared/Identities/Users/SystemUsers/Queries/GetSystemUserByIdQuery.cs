namespace Snebur.UseCases.Identities.Users.SystemUsers.Queries;

public record GetSystemUserByIdQuery(Guid Id) : GetEntityByIdQuery<UserResponse>(Id);
