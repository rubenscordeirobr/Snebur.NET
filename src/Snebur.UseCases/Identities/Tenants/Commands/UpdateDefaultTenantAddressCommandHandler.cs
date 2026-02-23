namespace Snebur.UseCases.Identities.Tenants.Commands;

public sealed class UpdateDefaultTenantAddressCommandHandler
    : CommandHandler<UpdateDefaultTenantAddressCommand, OperationResponse>
{
    private readonly IIdentityUnitOfWork _unitOfWork;

    public UpdateDefaultTenantAddressCommandHandler(
        IIdentityUnitOfWork unitOfWork)
    {
        Guard.NotNull(unitOfWork);

        _unitOfWork = unitOfWork;
    }

    protected override async Task<Result<OperationResponse>> HandleAsync(
        UpdateDefaultTenantAddressCommand command, CancellationToken cancellationToken)
    {
        Guard.NotNull(command);

        var tenant = await _unitOfWork.Tenants
            .GetByIdAsync(command.Tenant_Id, cancellationToken, t => t.DefaultAddress);

        if (tenant == null)
        {
            return Result.NotFoundFailure<OperationResponse>(
                 "Tenant.NotFound",
                 $"Tenant with id {command.Tenant_Id} not found.");
        }

        var address = command.Address;
        var newAddress = tenant.AddAddress(
             command.AddressName,
             address.Street,
             address.Number,
             address.Complement,
             address.Neighborhood,
             address.City,
             address.State,
             address.ZipCode,
             address.Country);

        tenant.SetDefaultAddress(newAddress);

        var result = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (result.IsFailure)
        {
            return Result.Failure<OperationResponse>(result.Error);
        }
        return Result.Success(new OperationResponse());
    }
}

