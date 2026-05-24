using Snebur.Core.Mappers;

namespace Snebur.UseCases.Identities.Tenants.Commands;

public sealed class CreateTenantAccountCommandHandler
    : CommandHandler<CreateTenantAccountCommand, CreateTenantAccountResponse>
{
    private readonly IIdentityUnitOfWork _unitOfWork;
    private readonly ISecureConfiguration _secureConfiguration;

    public CreateTenantAccountCommandHandler(
        IIdentityUnitOfWork unitOfWork,
        ISecureConfiguration secureConfiguration )
    {
        _unitOfWork = unitOfWork;
        _secureConfiguration = secureConfiguration;
    }

    protected override async Task<Result<CreateTenantAccountResponse>> HandleAsync(
        CreateTenantAccountCommand command, 
        CancellationToken cancellationToken)
    {
        Guard.NotNull(command);

        var salt = _secureConfiguration.GetPasswordSalt();
        var password = Password.Create(command.Password, salt);

        if (!password.IsSuccess)
        {
            return Result.Failure<CreateTenantAccountResponse>(password.Error);
        }

        var tenant = new Tenant(
            name: command.BusinessName,
            fiscalCode: command.FiscalCode,
            email: command.Email,
            businessType: command.BusinessType,
            currency: command.Currency,
            country: command.Country,
            language: command.Language,
            tenantState: TenantState.New,
            tenantStatus: TenantStatus.Active,
            tenantType: command.TenantType,
            phoneNumber: command.PhoneNumber,
            timeZoneOffset: TimeZoneOffset.Default);
 
       var user = tenant.CreateUser(
           name: command.Name,
           email: command.Email,
           language: command.Language,
           userState: UserState.Active,
           userStatus: UserStatus.Online,
           role: UserRole.Owner,
           phoneNumber: command.PhoneNumber,
           password.Value);

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            _unitOfWork.Add(tenant);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
           
            tenant.SetCreateOwner(user);

            _unitOfWork.Update(tenant);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            var response = new CreateTenantAccountResponse(tenant.Id);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(ex, cancellationToken);

            var error = HttpErrorMapper.MapExceptionToError(ex,
                "CreateTenantCommandHandler.HandleAsync");
            return Result.Failure<CreateTenantAccountResponse>(error);
        }
    }
}
