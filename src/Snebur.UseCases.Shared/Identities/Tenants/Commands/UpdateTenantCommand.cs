namespace Snebur.UseCases.Identities.Tenants.Commands;

public record UpdateTenantCommand : CommandRequest<OperationResponse> 
{
    public required Guid Tenant_Id { get; init; }
    public required string Name { get; init; }
    public required FiscalCode FiscalCode { get; init; }
    public required Country Country { get; init; }
    public required Language Language { get; init; }
    public required Currency Currency { get; init; }
    public required BusinessType BusinessType { get; init; }
    public required TenantType TenantType { get; init; }
}
