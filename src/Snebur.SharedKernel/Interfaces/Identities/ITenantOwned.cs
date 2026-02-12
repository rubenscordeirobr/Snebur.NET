namespace Snebur.SharedKernel.Interfaces.Identities;

public interface ITenantOwned
{
    Guid Tenant_Id { get; }
}
