using Snebur.SharedKernel.Interfaces.Identities;
using Snebur.SharedKernel.Models.Identities;

namespace Snebur.SharedKernel.Constants;

public static class DefaultAdminUserConstants
{
    public const string Email = "admin@snebur.com.br";
    public const string Name = "Admin";
    public const string PhoneNumber = "+5542999999999";
    public const string TestPassword = "Admin@Teste%#";

#pragma warning disable CS9264, IDE1006

    public static readonly Guid User_Id = new("57d4e3b2-8903-49f9-b406-5f6c2b619fb5");

    public static IUser User
        => field ??= CreateSuperAdminUser();

#pragma warning restore CS9264

    private static CachedUser CreateSuperAdminUser()
    {
        return CachedUser.Create(
            User_Id,
            Name,
            Email,
            VerificationState.Verified,
            VerificationState.Verified,
            new PhoneNumber(PhoneNumber),
            Language.Default,
            UserRole.Admin,
            UserType.AdminUser
        );
    }
}
