using Snebur.UseCases.Mappers.Identities;

namespace Snebur.UseCases.Identities.Tenants.Queries;

public sealed class GetTenantByIdQueryHandler : IGetQueryResultHandler<GetTenantByIdQuery, TenantResponse>
{
    private readonly ITenantRepository _tenantRepository;

    public GetTenantByIdQueryHandler(
        ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<Result<TenantResponse>> HandleAsync(
        GetTenantByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        Guard.NotNull(query);

        var tenant = await _tenantRepository.GetByIdAsync(query.Id,
            cancellationToken,
            t => t.DefaultAddress);

        if (tenant is null)
        {
            return Result.NotFoundFailure<TenantResponse>(
                "Tenant.NotFound",
                $"Tenant with id {query.Id} not found.");
        }

        var tenantResponse = TenantMapper.ToResponse(tenant);
        return Result.Success(tenantResponse);
    }
}
