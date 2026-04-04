namespace Snebur.UseCases.Abstractions.Identities;

public interface IAdminUserService  : ICommunicationService
{
    #region Queries

    Task<Result<UserResponse>> GetAdminUserByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Result<UserResponse>> GetAdminUserByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<Result<UserResponse>> GetAdminUserByPhoneNumberAsync(
        string phoneNumber,
        CancellationToken cancellationToken = default);

    Task<Result<UserResponse>> GetAdminUserByEmailOrPhoneNumberAsync(
        string emailOrPhoneNumber,
        CancellationToken cancellationToken = default);

    #endregion
}
