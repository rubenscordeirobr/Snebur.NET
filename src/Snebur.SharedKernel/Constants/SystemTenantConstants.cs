using Snebur.SharedKernel.Interfaces.Identities;
using Snebur.SharedKernel.Models.Identities;

namespace Snebur.SharedKernel.Constants;

public static class SystemTenantConstants
{
    public const string Email = "systemtenant@snebur.com.br";
    public const string Name = "System Tenant";

    public static readonly Guid Tenant_Id = new("F4D70ACF-1232-49AD-1FD6-038540DF9F0F");
    public static readonly Guid User_Id = new("F4D70ACF-1232-49AD-1FD6-038540DF9F0F");

    public const string FiscalCode = "48302193000161";
    public const string PhoneNumber = "+554299997406";
    public const string TestPassword = "TenantAdmin@Teste%#";

#pragma warning disable CS9264

    public static IUser OwnerUser
         => field ??= CreateSystemTenantOwnerUser();

#pragma warning restore CS9264

    private static CachedUser CreateSystemTenantOwnerUser()
    {
        return CachedUser.Create(
            User_Id,
            Name,
            Email,
            VerificationState.Verified,
            VerificationState.Verified,
            new PhoneNumber(PhoneNumber),
            Language.Default,
            UserRole.Owner,
            UserType.TenantUser
        );
    }
}
