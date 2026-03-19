namespace Snebur.UseCases.Identities.Users.TenantUsers.Commands;

public class CreateTenantUserCommandHandler : CommandHandler<CreateTenantUserCommand, CreateTenantUserResponse>
{
    private readonly IIdentityUnitOfWork _unitOfWork;
    private readonly ISecureConfiguration _secureConfiguration;

    public CreateTenantUserCommandHandler(
        IIdentityUnitOfWork unitOfWork,
        ISecureConfiguration secureConfiguration)
    {
        _unitOfWork = unitOfWork;
        _secureConfiguration = secureConfiguration;
    }

    protected override async Task<Result<CreateTenantUserResponse>> HandleAsync(
        CreateTenantUserCommand command,
        CancellationToken cancellationToken)
    {
        Guard.NotNull(command);

        var tenant = await _unitOfWork.Tenants.GetByIdAsync(command.Tenant_Id);
        if (tenant is null)
        {
            return Result.Failure<CreateTenantUserResponse>(
                new NotFoundError("TenantNotFound", "Tenant not found."));
        }

        var salt = _secureConfiguration.GetPasswordSalt();
        var password = Password.Create(command.Password, salt);
        if (password.IsFailure)
        {
            return Result.Failure<CreateTenantUserResponse>(password.Error);
        }

        var tenantUser = tenant.CreateUser(
            name: command.Name,
            email: command.Email,
            language: tenant.Language,
            role: command.Role,
            userState: UserState.New,
            userStatus: UserStatus.New,
            phoneNumber: command.PhoneNumber,
            password: password.Value);

        _unitOfWork.Update(tenant);
         
        var result = await _unitOfWork.SaveChangesAsync(silent: true, cancellationToken);
        if (!result.IsSuccess)
        {
            return Result.Failure<CreateTenantUserResponse>(result.Error);
        }
        Guard.NotEmpty(tenantUser.Id);
        return Result.Success(new CreateTenantUserResponse(tenantUser.Id));
    }
}

