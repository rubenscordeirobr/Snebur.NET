using Snebur.Application.Models.Security;

namespace Snebur.Application.Abstractions.Security;

public interface IAuthenticationAttemptLimiterService
{
    Task IncrementFailedAttemptsAsync(
        string ipAddress, 
        CancellationToken cancellationToken = default);

    Task<MaxAuthenticationResult> MaxAuthenticationReachedAsync(
            string ipAddress, 
            CancellationToken cancellationToken = default);
}
