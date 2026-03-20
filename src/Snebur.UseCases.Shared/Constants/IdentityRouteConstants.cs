namespace Snebur.UseCases.Constants;

 
    public static class IdentityRouteConstants
{
    private const string IdentityBase = $"{RouteConstants.Api}/identity";
     
    public const string Tenants = $"{IdentityBase}/tenants";
    
    public const string TenantValidation = $"{IdentityBase}/tenant-validation";
    public const string TenantUserValidation = $"{IdentityBase}/tenant-users-validation";
    public const string TenantUserAuthentication = $"{IdentityBase}/tenant-user-authentication";
    public const string TenantUserAuthenticationValidation = $"{IdentityBase}/tenant-user-authentication-validation";

    public const string AdminUserAuthentication = $"{IdentityBase}/admin-user-authentication";
    public const string AdminUserAuthenticationValidation = $"{IdentityBase}/admin-user-authentication-validation";

    public const string TenantAddress = $"{IdentityBase}/tenant-address";
    public const string TenantUpdateDefaultAddress = "update-default-address";

    public const string TenantUsersRoute = $"{IdentityBase}/tenant-users";
    public const string AdminUsersRoute = $"{IdentityBase}/admin-users";

    //TenantUserAuthenticationService
    public const string Login = "login";
    public const string Logout = "logout";
    public const string CreateTenantAccount = "create-tenant-account";
}
