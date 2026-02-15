namespace Snebur.Persistence.Identity.Repositories;

internal class TenantRepository : RepositoryBase<Tenant>, ITenantRepository
{
    public TenantRepository(
        IdentityDbContext dbContext,
        IHttpContextSessionAccessor userSessionAccessor,
        TrackingOption trackingOption = TrackingOption.NoTracking)
        : base(dbContext, userSessionAccessor, trackingOption)
    {
    }

    protected override bool ShouldFilterTenantOwned()
    {
        return false;
    }

    public Task<Tenant?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken)
        => FindAsync(x => x.Name == name, cancellationToken);

    public Task<bool> EmailExistsAsync(
        string email,
        CancellationToken token)
    {
        return AnyAsync(x => x.Email == email, token);
    }

    public Task<bool> EmailExistsAsync(
        string email,
        Guid currentTenant_Id,
        CancellationToken token)
    {
        return AnyAsync(x => x.Email == email && x.Id != currentTenant_Id, token);
    }

    public Task<bool> FiscalCodeExistsAsync(
        string fiscalCode,
        CancellationToken token)
    {
        return AnyAsync(x => x.FiscalCode == fiscalCode, token);
    }

    public Task<bool> FiscalCodeExistsAsync(
        string fiscalCode,
        Guid currentTenant_Id,
        CancellationToken token)
    {
        return AnyAsync(x => x.FiscalCode == fiscalCode && x.Id != currentTenant_Id, token);
    }

    public Task<bool> PhoneNumberExitsAsync(
        string phoneNumber,
        CancellationToken token)
    {
        return AnyAsync(x => x.PhoneNumber == phoneNumber, token);
    }

    public Task<bool> PhoneNumberExitsAsync(
        string phoneNumber,
        Guid currentTenant_Id,
        CancellationToken token)
    {
        return AnyAsync(x => x.PhoneNumber == phoneNumber && x.Id != currentTenant_Id, token);
    }
}
