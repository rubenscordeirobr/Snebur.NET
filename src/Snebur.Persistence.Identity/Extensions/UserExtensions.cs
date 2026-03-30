namespace Snebur.Persistence.Identity.Extensions;

internal static class UserExtensions
{
    internal static void SetAnonymousId(this SystemUser user)
    {
        Guard.NotNull(user);

        if (user.Id != Guid.Empty)
        {
            throw new InvalidOperationException("Id is already set.");
        }

        user.SetPropertyValue(p => p.Id, AnonymousUserConstants.User_Id);
        user.SetPropertyValue(p => p.Email, AnonymousUserConstants.Email);
        user.SetPropertyValue(p => p.Name, AnonymousUserConstants.Name);
        user.SetPropertyValue(p => p.UserStatus, UserStatus.Anonymous);
        user.SetPropertyValue(p => p.UserState, UserState.Active);
        
        user.SetCreateSession(AnonymousUserConstants.Session_Id);
    }
}
