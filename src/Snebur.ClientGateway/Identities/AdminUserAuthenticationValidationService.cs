namespace Snebur.ClientGateway.Identities;

[Route(IdentityRouteConstants.AdminUserAuthenticationValidation)]
public class AdminUserAuthenticationValidationService : IAdminUserAuthenticationValidationService
{
    private readonly IHttpClientMediator<AdminUserAuthenticationValidationService> _mediator;

    public AdminUserAuthenticationValidationService(IHttpClientMediator<AdminUserAuthenticationValidationService> mediator)
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

    public Task<bool> VerifyAdminUserCredentialsAsync(
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
