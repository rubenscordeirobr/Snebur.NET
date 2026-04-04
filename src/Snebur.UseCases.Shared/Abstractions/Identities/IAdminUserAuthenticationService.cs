using Snebur.UseCases.Identities.Authentications.Commands;

namespace Snebur.UseCases.Abstractions.Identities;

public interface IAdminUserAuthenticationService : ICommunicationService
{
    Task<Result<AdminUserLoginResponse>> LoginAsync(
        AdminUserLoginCommand command,
        CancellationToken cancellationToken = default);

    Task<Result<OperationResponse>> LogoutAsync(
        AdminUserLogoutCommand command,
        CancellationToken cancellationToken = default);
}
