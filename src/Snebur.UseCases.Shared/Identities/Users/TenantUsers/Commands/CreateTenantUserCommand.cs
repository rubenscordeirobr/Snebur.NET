namespace Snebur.UseCases.Identities.Users.TenantUsers.Commands;

public record CreateTenantUserCommand : CommandRequest<CreateTenantUserResponse>
{
    public required Guid Tenant_Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required PhoneNumber PhoneNumber { get; init; }
    public required UserRole Role { get; init; }
}

public record CreateTenantUserResponse(Guid Id) : IResponse;
