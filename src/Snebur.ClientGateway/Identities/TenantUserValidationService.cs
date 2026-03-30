namespace Snebur.ClientGateway.Identities;

[Route(IdentityRouteConstants.TenantUserValidation)]
public class TenantUserValidationService : ITenantUserValidationService
{
    private readonly IHttpClientMediator<TenantUserValidationService> _mediator;

    public TenantUserValidationService(IHttpClientMediator<TenantUserValidationService> mediator)
    {
        _mediator = mediator;
    }

    public Task<bool> IsEmailUniqueAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return _mediator.IsValidAsync(
            [nameof(email)],
            [email],
            cancellationToken);
    }

    public Task<bool> IsEmailUniqueAsync(
        Guid currentUser_Id,
        string email,
        CancellationToken cancellationToken = default)
    {
        return _mediator.IsValidAsync(
            [nameof(currentUser_Id), nameof(email)],
            [currentUser_Id, email],
            cancellationToken);
    }

    public Task<bool> IsPhoneNumberUniqueAsync(
        string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        return _mediator.IsValidAsync(
            [nameof(phoneNumber)],
            [phoneNumber],
            cancellationToken);
    }

    public Task<bool> IsPhoneNumberUniqueAsync(
        Guid currentUser_Id,
        string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        return _mediator.IsValidAsync(
            [nameof(currentUser_Id), nameof(phoneNumber)],
            [currentUser_Id, phoneNumber],
            cancellationToken);
    }
}
