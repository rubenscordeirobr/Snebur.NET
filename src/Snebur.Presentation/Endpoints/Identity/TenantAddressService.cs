using Snebur.UseCases.Identities.Tenants.Commands;

namespace Snebur.Presentation.Endpoints.Identity;

[EndPoint(IdentityRouteConstants.TenantAddress)]
public class TenantAddressService : ApiEndpointBase, ITenantAddressService
{
    private readonly IRequestMediator _mediator;

    public TenantAddressService(IRequestMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost(IdentityRouteConstants.TenantUpdateDefaultAddress)]
    public Task<Result<OperationResponse>> UpdateDefaultAddressAsync(
         UpdateDefaultTenantAddressCommand command,
         CancellationToken cancellationToken = default)
    {
        return _mediator.RunAsync(command, cancellationToken);
    }
}
