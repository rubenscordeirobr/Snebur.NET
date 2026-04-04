namespace Snebur.UseCases.Abstractions.Identities;

public interface ITenantService : ICommunicationService
{
    #region Queries
    Task<Result<TenantResponse>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    #endregion

    #region Commands

    Task<Result<CreateTenantAccountResponse>> CreateAsync(
        CreateTenantAccountCommand command,
        CancellationToken cancellationToken = default);

    Task<Result<OperationResponse>> UpdateAsync(
        UpdateTenantCommand command,
        CancellationToken cancellationToken = default);

    Task<Result<OperationResponse>> DeleteAsync(
        DeleteTenantCommand command,
        CancellationToken cancellationToken = default);

    #endregion
}

public interface ITenantAddressService : ICommunicationService
{
    Task<Result<OperationResponse>> UpdateDefaultAddressAsync(
        UpdateDefaultTenantAddressCommand command,
        CancellationToken cancellationToken = default);
}
