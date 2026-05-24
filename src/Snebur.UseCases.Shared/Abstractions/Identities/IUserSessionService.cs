using Snebur.UseCases.Identities.UserSessions;

namespace Snebur.UseCases.Abstractions.Identities;

public interface IUserSessionService
{
    Task ChangeLanguageAsync(
        ChangeUserSessionLanguageCommand command, 
        CancellationToken cancellationToken = default);
}
