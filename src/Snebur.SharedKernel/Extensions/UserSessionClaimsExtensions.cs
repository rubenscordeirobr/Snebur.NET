using System.Security.Claims;
using Snebur.SharedKernel.Constants;
using Snebur.SharedKernel.Models.Security;

namespace Snebur.SharedKernel.Extensions;

public static class UserSessionClaimsExtensions
{
    public static ClaimsPrincipal ToClaimsPrincipal(this UserSessionClaims userSessionClaims)
    {

        if (userSessionClaims is null || userSessionClaims.IsAnonymous())
        {
            return new ClaimsPrincipal(new ClaimsIdentity());
        }

        var claims = userSessionClaims.GetClaims();
        var identity = new ClaimsIdentity(claims, authenticationType: TenantUserAuthenticationConfig.AuthenticationScheme);
        return new ClaimsPrincipal(identity);
    }

    public static Claim[] GetClaims(this UserSessionClaims userSessionClaims)
    {
        Guard.NotNull(userSessionClaims);

        return [
            new Claim(UserSessionClaimTypes.SessionId, userSessionClaims.Session_Id.ToString()),
            new Claim(ClaimTypes.Name, userSessionClaims.Name),
            new Claim(ClaimTypes.Email, userSessionClaims.Email),
            new Claim(ClaimTypes.MobilePhone, userSessionClaims.PhoneNumber),
            new Claim(ClaimTypes.IsPersistent, userSessionClaims.IsPersistent.ToString()),
            new Claim(UserSessionClaimTypes.Language, userSessionClaims.Language.ToString()),
            new Claim(ClaimTypes.Role, userSessionClaims.UserRole.ToString()),
            new Claim(UserSessionClaimTypes.UserType, userSessionClaims.UserType.ToString()),
        ];
    }

    public static bool IsAnonymous(this UserSessionClaims userSessionClaims)
    {
        Guard.NotNull(userSessionClaims);

        return userSessionClaims.UserType == UserType.Anonymous;
    }
}
