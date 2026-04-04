using Snebur.SharedKernel.Models.Security;

namespace Snebur.Application.Abstractions.Services;

public interface IUserSessionTokenHandler : IApplicationService
{
    string WriteToken(UserSessionClaims userSessionClaims, bool isPersistent);

    UserSessionClaims? ReadToken(string token);
}
