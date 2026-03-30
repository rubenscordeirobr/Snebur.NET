using Snebur.Application.Abstractions.Persistence.Identities;

namespace Snebur.Application.Extensions;

public static class IdentityUnitOfWorkExtensions
{
    public static async Task<User?> GetUserAsync(
         this IIdentityUnitOfWork unitOfWork,
         Guid user_Id, UserType userType)
    {
        Guard.NotNull(unitOfWork);

        return userType switch
        {
            UserType.SystemUser => await unitOfWork.SystemUsers.GetByIdAsync(user_Id),
            UserType.TenantUser => await unitOfWork.TenantUsers.GetByIdAsync(user_Id),
            UserType.AdminUser => await unitOfWork.AdminUsers.GetByIdAsync(user_Id),
            _ => throw new InvalidOperationException($"User type {userType} not supported")
        };
    }
}
