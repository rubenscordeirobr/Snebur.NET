
namespace Snebur.Infrastructure.Helpers;

internal static class SecureConfigKeysProvider
{
    private const string PasswordSaltKey = "PasswordSalt";
    private const string JwtAuthenticationKey = "JwtAuthentication";
    private const string JwtAudienceKey = "JwtAudience";
    private const string JwtIssuerKey = "JwtIssuer";

    internal static SecureConfigKeyPair PasswordSalt
         => CreateSecureConfigKeys(PasswordSaltKey);

    internal static SecureConfigKeyPair JwtAuthentication
        => CreateSecureConfigKeys(JwtAuthenticationKey);

    internal static SecureConfigKeyPair JwtAudience
        => CreateSecureConfigKeys(JwtAudienceKey);

    internal static SecureConfigKeyPair JwtIssuer =>
        CreateSecureConfigKeys(JwtIssuerKey);

    private static SecureConfigKeyPair CreateSecureConfigKeys(string passwordSalt)
    {
        var appSettingsKey = SecureConfigHelper.BuildConfigurationKey(passwordSalt);
        var environmentKey = SecureConfigHelper.BuildEnvironmentKey(passwordSalt);
        return new SecureConfigKeyPair(appSettingsKey, environmentKey);
    }
}

internal record SecureConfigKeyPair(
    string AppSettingsKey,
    string EnvironmentKey);
