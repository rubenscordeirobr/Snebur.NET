namespace Snebur.Persistence.Identity.Repositories;

internal class AdminUserRepository : UserRepository<AdminUser>, IAdminUserRepository
{
    public AdminUserRepository(
        IdentityDbContext dbContext,
        IHttpContextSessionAccessor userSessionAccessor,
        TrackingOption trackingOption = TrackingOption.NoTracking)
        : base(dbContext, userSessionAccessor, trackingOption)
    {
    }
}
