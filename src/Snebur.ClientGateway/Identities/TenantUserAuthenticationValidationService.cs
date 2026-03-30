namespace Snebur.ClientGateway.Identities;

[Route(IdentityRouteConstants.TenantUserAuthenticationValidation)]
public class TenantUserAuthenticationValidationService : ITenantUserAuthenticationValidationService
{
    private readonly IHttpClientMediator<TenantUserAuthenticationValidationService> _mediator;

    public TenantUserAuthenticationValidationService(IHttpClientMediator<TenantUserAuthenticationValidationService> mediator)
    {
        _mediator = mediator;
    }

    public Task<bool> EmailOrPhoneNumberExitsAsync(
        string emailOrPhoneNumber, 
        CancellationToken cancellationToken = default)
    {
        return _mediator.IsValidAsync(
            [nameof(emailOrPhoneNumber)],
            [emailOrPhoneNumber],
            cancellationToken);
    }

    public Task<bool> VerifyTenantUserCredentialsAsync(
        string emailOrPhoneNumber, 
        string password, 
        CancellationToken cancellationToken = default)
    {
        return _mediator.IsValidAsync(
            [nameof(emailOrPhoneNumber), nameof(password)],
            [emailOrPhoneNumber, password],
            cancellationToken);
    }
}
