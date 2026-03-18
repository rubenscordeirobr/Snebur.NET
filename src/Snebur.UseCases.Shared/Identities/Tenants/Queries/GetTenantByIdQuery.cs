namespace Snebur.UseCases.Identities.Tenants.Queries;

public record GetTenantByIdQuery(Guid Id)
    : QueryRequest<TenantResponse>;
