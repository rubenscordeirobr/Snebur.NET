namespace Snebur.Application.Abstractions.Services;

public interface IEntityAuthorizationService : IApplicationService
{
    /// <exception cref="ForbiddenSecurityException"></exception>
    void ValidateEntityChange(
         EntityBase entity,
         IUserSession userSession,
         EntityChangeState entityChangeState);
}
