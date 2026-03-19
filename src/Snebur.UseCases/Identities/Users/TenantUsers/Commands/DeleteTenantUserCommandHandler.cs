namespace Snebur.UseCases.Identities.Users.TenantUsers.Commands;

public sealed class DeleteTenantUserCommandHandler
    : CommandHandler<DeleteTenantUserCommand, OperationResponse>
{
    private readonly IIdentityUnitOfWork _unitOfWork;

    public DeleteTenantUserCommandHandler(
        IIdentityUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    protected override async Task<Result<OperationResponse>> HandleAsync(
        DeleteTenantUserCommand command,
        CancellationToken cancellationToken)
    {
        Guard.NotNull(command);

        var tenantUser = await _unitOfWork.TenantUsers
            .GetByIdAsync(command.Id, cancellationToken);

        if (tenantUser is null)
        {
            return Result.NotFoundFailure<OperationResponse>(
                "TenantUser.NotFound",
                $"TenantUser with id {command.Id} not found.");
        }

        _unitOfWork.Delete(tenantUser);

        var result = await _unitOfWork.SaveChangesAsync(silent: true, cancellationToken);
        if (result.IsFailure)
        {
            return Result.Failure<OperationResponse>(result.Error);
        }
        return Result.Success(new OperationResponse());
    }
}
