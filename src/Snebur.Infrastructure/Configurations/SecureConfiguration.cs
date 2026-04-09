using Snebur.Application.Abstractions.Security;
using Snebur.Core.Exceptions;
using Snebur.Core.Helpers;
using Snebur.Core.Utils;
using Snebur.Infrastructure.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Snebur.Infrastructure.Configurations;

public class SecureConfiguration : ISecureConfiguration
{
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _hostEnvironment;

    public SecureConfiguration(
        IConfiguration configuration,
        IHostEnvironment hostEnvironment)
    {
        _configuration = configuration;
        _hostEnvironment = hostEnvironment;
    }

    public string GetPasswordSalt()
        => GetConfigurationValue(SecureConfigKeysProvider.PasswordSalt);

    public string GetAuthenticationKey()
        => GetConfigurationValue(SecureConfigKeysProvider.JwtAuthentication);

    public string GetJwtAudience()
        => GetConfigurationValue(SecureConfigKeysProvider.JwtAudience);

    public string GetJwtIssuer()
       => GetConfigurationValue(SecureConfigKeysProvider.JwtIssuer);

    private string GetConfigurationValue(SecureConfigKeyPair secureConfigKeyPair)
    {
        if (_hostEnvironment.IsDevelopment() || EnvironmentHelper.IsXUnitTesting())
        {
            return "dev-" + CaseConventionUtils.ToSnakeCase(secureConfigKeyPair.EnvironmentKey);
        }
         
        return _configuration[secureConfigKeyPair.AppSettingsKey] ??
               Environment.GetEnvironmentVariable(secureConfigKeyPair.EnvironmentKey) ??
               throw new MissingConfigurationException("Authentication key is not configured properly.");
    }

  
}
