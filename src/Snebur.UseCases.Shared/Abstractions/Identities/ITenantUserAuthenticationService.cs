using Snebur.UseCases.Identities.Authentications.Commands;

namespace Snebur.UseCases.Abstractions.Identities;

public interface ITenantUserAuthenticationService : ICommunicationService
{
    Task<Result<TenantUserLoginResponse>> LoginAsync(
        TenantUserLoginCommand command,
        CancellationToken cancellationToken = default);

    Task<Result<OperationResponse>> LogoutAsync(
        TenantUserLogoutCommand command,
        CancellationToken cancellationToken = default);

    Task<Result<CreateTenantAccountResponse>> CreateTenantAccountAsync(
       CreateTenantAccountCommand command,
       CancellationToken cancellationToken = default);

}
