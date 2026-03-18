namespace Snebur.UseCases.Identities.Tenants.Commands;

public sealed class DeleteTenantCommandHandler
    : CommandHandler<DeleteTenantCommand, OperationResponse>
{
    private readonly IIdentityUnitOfWork _unitOfWork;

    public DeleteTenantCommandHandler(
        IIdentityUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    protected override async Task<Result<OperationResponse>> HandleAsync(
        DeleteTenantCommand command,
        CancellationToken cancellationToken)
    {
        Guard.NotNull(command);

        var tenant = await _unitOfWork.Tenants
            .GetByIdAsync(command.Tenant_Id, cancellationToken);

        if (tenant is null)
        {
            return Result.NotFoundFailure<OperationResponse>(
                "Tenant.NotFound",
                $"Tenant with id {command.Tenant_Id} not found.");
        }

        _unitOfWork.Delete(tenant);

        var result = await _unitOfWork.SaveChangesAsync(silent: true, cancellationToken);
        if (result.IsFailure)
        {
            return Result.Failure<OperationResponse>(result.Error);
        }
        return Result.Success(new OperationResponse());
    }
}
