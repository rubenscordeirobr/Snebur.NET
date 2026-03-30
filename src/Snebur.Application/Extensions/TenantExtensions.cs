namespace Snebur.Application.Extensions;

public static class TenantExtensions
{
    public static bool IsActive(this Tenant tenant)
    {
        Guard.NotNull(tenant);

        return tenant.TenantState is
            TenantState.New or
            TenantState.Onboarding or
            TenantState.Trial or
            TenantState.System or
            TenantState.Operational;
    }
}
