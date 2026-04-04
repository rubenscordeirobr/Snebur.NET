namespace Snebur.Application.Abstractions.Security;
public interface ISecureConfiguration
{
    string GetAuthenticationKey();
    string GetJwtAudience();
    string GetJwtIssuer();
    string GetPasswordSalt();
}
