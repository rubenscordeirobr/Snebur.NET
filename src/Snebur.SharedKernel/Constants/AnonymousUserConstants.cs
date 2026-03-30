using Snebur.Core.Infos;
using Snebur.SharedKernel.Interfaces.Identities;
using Snebur.SharedKernel.Models.Identities;

namespace Snebur.SharedKernel.Constants;
public static class AnonymousUserConstants
{
    public const string Email = "anonymous@snebur.com";
    public const string Name = "Anonymous";
    public const string PhoneNumber = "+5542900000000";

    public static readonly Guid Session_Id = new("49AD1FD6-0385-40DF-9F0F-538B61065442");
    public static readonly Guid User_Id = new("FB8A6865-A999-0000-8BB7-F4D70ACF1232");

#pragma warning disable CS9264 
      
    public static IUser AnonymousUser
        => field ??= CreateAnonymousUser();

    public static IUserSession AnonymousUserSession
        => field ??= CreateAnonymousUserSession();

#pragma warning restore CS9264

    private static CachedUser CreateAnonymousUser()
    {
        return CachedUser.Create(
            User_Id,
            Name,
            Email,
            VerificationState.Verified,
            VerificationState.Verified,
            new PhoneNumber(PhoneNumber),
            Language.Default,
            UserRole.Anonymous,
            UserType.Anonymous
        );
    }

    private static CachedUserSession CreateAnonymousUserSession()
    {
        var headInfo = ClientRequestHeaderInfo.System;

        return CachedUserSession.Create(
            id: Session_Id,
            applicationName: headInfo.ApplicationName,
            ipAddress: headInfo.IpAddress,
            userAgent: headInfo.UserAgent,
            isActive: true,
            isPersistent: true,
            lastActivity: DateTime.UtcNow,
            startedAt: DateTime.UtcNow,
            terminatedAt: null,
            language: Language.Default,
            authenticationType: AuthenticationType.Anonymous,
            terminationReason: null,
            userRole: UserRole.Anonymous,
            userType: UserType.Anonymous,
            user_Id: User_Id,
            tenant_Id: null,
            geoLocation: null);
    }
}
