using Snebur.UseCases.Identities.Users.TenantUsers.Commands;

namespace Snebur.UseCases.Abstractions.Identities;

public interface ITenantUserService : ICommunicationService
{
    #region Queries

    Task<Result<UserResponse>> GetTenantUserByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Result<UserResponse>> GetTenantUserByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<Result<UserResponse>> GetTenantUserByPhoneNumberAsync(
        string phoneNumber,
        CancellationToken cancellationToken = default);

    Task<Result<UserResponse>> GetTenantUserByEmailOrPhoneNumberAsync(
        string emailOrPhoneNumber,
        CancellationToken cancellationToken = default);

    #endregion

    #region Commands

    Task<Result<CreateTenantUserResponse>> CreateTenantUserAsync(
        CreateTenantUserCommand command,
        CancellationToken cancellationToken = default);

    Task<Result<OperationResponse>> DeleteTenantUserAsync(
        DeleteTenantUserCommand command,
        CancellationToken cancellationToken = default);

    #endregion
}
