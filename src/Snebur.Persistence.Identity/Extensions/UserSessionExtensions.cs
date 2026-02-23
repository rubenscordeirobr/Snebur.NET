namespace Snebur.Persistence.Identity.Extensions;

internal static class UserSessionExtensions
{
    internal static void SetAnonymousSystemSessionId(this UserSession session)
    {
        if (session.Id != Guid.Empty)
        {
            throw new InvalidOperationException("Id is already set.");
        }
                
        session.SetPropertyValue(p => p.Id, AnonymousUserConstants.Session_Id);
        session.SetPropertyValue(p => p.AuthenticationType, AuthenticationType.Anonymous);
        session.SetPropertyValue(p => p.Tenant_Id, null);
        session.SetCreateSession(AnonymousUserConstants.Session_Id);
    }
}
