namespace Snebur.RuntimeServices.Abstractions;

internal interface IUserSessionVerificationServiceTest
{
    Task<UserSession> CreateAnonymousSessionAsync();
}
