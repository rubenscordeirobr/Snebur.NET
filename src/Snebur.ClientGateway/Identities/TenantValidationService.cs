namespace Snebur.ClientGateway.Identities;

[Route(IdentityRouteConstants.TenantValidation)]
public class TenantValidationService : ITenantValidationService
{
    private readonly IHttpClientMediator<TenantValidationService> _mediator;

    public TenantValidationService(IHttpClientMediator<TenantValidationService> mediator)
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
        Guid currentTenant_Id,
        Guid currentTenantOwner_Id,
        string email,
        CancellationToken cancellationToken = default)
    {
        return _mediator.IsValidAsync(
            [nameof(currentTenant_Id), nameof(currentTenantOwner_Id), nameof(email)],
            [currentTenant_Id, currentTenantOwner_Id, email],
            cancellationToken);
    }

    public Task<bool> IsFiscalCodeUniqueAsync(
        string fiscalCode,
        CancellationToken cancellationToken = default)
    {
        return _mediator.IsValidAsync(
            [nameof(fiscalCode)],
            [fiscalCode],
            cancellationToken);
    }

    public Task<bool> IsFiscalCodeUniqueAsync(
        Guid currentTenant_Id,
        string fiscalCode,
        CancellationToken cancellationToken = default)
    {
        return _mediator.IsValidAsync(
            [nameof(currentTenant_Id), nameof(fiscalCode)],
            [currentTenant_Id, fiscalCode],
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
        Guid currentTenant_Id,
        Guid currentTenantOwner_Id,
        string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        return _mediator.IsValidAsync(
            [nameof(currentTenant_Id), nameof(currentTenantOwner_Id), nameof(phoneNumber)],
            [currentTenant_Id, currentTenantOwner_Id, phoneNumber],
            cancellationToken);
    }
}
