namespace Snebur.SharedKernel.Constants;

public static class TenantUserAuthenticationConfig
{
    public const string AuthenticationScheme = "TenantUserAuthentication";
    public const string LoginRoute = "/{culture}/login";
    public const string LogoutRoute = "/{culture}/logout";
    public const string AccessDeniedRoute = "/access-denied";
}
