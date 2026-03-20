namespace Snebur.Testing.Core.Mocks.Infrastructure;

public sealed class SecureConfigurationMock: ISecureConfiguration
{
    public string GetPasswordSalt()
        => "test-salt";

    public string GetAuthenticationKey()
        => "test-key";
     
    public string GetJwtAudience()
        => "test-audience";

    public string GetJwtIssuer()
        => "test-issuer";
}
