namespace Snebur.Persistence.Identity.Repositories;

internal class UserSessionRepository : RepositoryBase<UserSession>, IUserSessionRepository
{
    public UserSessionRepository(
        IdentityDbContext dbContext,
        IHttpContextSessionAccessor userSessionAccessor,
        TrackingOption trackingOption = TrackingOption.NoTracking)
        : base(dbContext, userSessionAccessor, trackingOption)
    {
    }

    
    public Task<UserSession?> GetByIdWithUserAsync(
        Guid session_Id,
        CancellationToken cancellationToken = default)
    {
        return GetByIdAsync(session_Id,
                cancellationToken,
                userSession => userSession.User);
    }
}
