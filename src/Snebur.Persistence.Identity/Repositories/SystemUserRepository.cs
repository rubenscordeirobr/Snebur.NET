namespace Snebur.Persistence.Identity.Repositories;

internal class SystemUserRepository : UserRepository<SystemUser>, ISystemUserRepository
{
    public SystemUserRepository(
        IdentityDbContext dbContext,
        IHttpContextSessionAccessor userSessionAccessor,
        TrackingOption trackingOption = TrackingOption.NoTracking)
        : base(dbContext, userSessionAccessor, trackingOption)
    {
    }
}
