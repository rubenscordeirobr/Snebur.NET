namespace Snebur.UseCases.Identities.Tenants.Commands;

public class UpdateTenantCommandHandler
     : CommandHandler<UpdateTenantCommand, OperationResponse>
{
    private readonly IIdentityUnitOfWork _unitOfWork;

    public UpdateTenantCommandHandler(IIdentityUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    protected override async Task<Result<OperationResponse>> HandleAsync(
        UpdateTenantCommand command,
        CancellationToken cancellationToken)
    {
        Guard.NotNull(command);

        var tenant = await _unitOfWork.Tenants.GetByIdAsync(command.Tenant_Id, cancellationToken);
        if (tenant is null)
        {
            return Result.NotFoundFailure<OperationResponse>(
                "Tenant.NotFound",
                 $"Tenant with id {command.Tenant_Id} not found.");
        }

        tenant.Update(
            name: command.Name,
            country: command.Country,
            language: command.Language,
            currency: command.Currency,
            businessType: command.BusinessType,
            tenantType: command.TenantType,
            fiscalCode: command.FiscalCode);

        _unitOfWork.Update(tenant);

        var result = await _unitOfWork.SaveChangesAsync(silent: true, cancellationToken);
        if (result.IsFailure)
        {
            return Result.Failure<OperationResponse>(result.Error);
        }
        return Result.Success(new OperationResponse());
    }
}

