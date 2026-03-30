using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Snebur.Application.Abstractions.Security;
using Snebur.SharedKernel.Configuration;
using Snebur.SharedKernel.Extensions;
using Snebur.SharedKernel.Factories;
using Snebur.SharedKernel.Models.Security;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Snebur.RuntimeServices.Services;

public class UserSessionTokenHandler : IUserSessionTokenHandler
{
    private readonly JwtSecurityTokenHandler _tokenHandler = new();
    private readonly TokenValidationParameters _validationParameters;
    private readonly SigningCredentials _credentials;

    private readonly ILogger<UserSessionTokenHandler> _logger;

    public UserSessionTokenHandler(
        ISecureConfiguration secureConfiguration,
        ILogger<UserSessionTokenHandler> logger)
    {
        Guard.NotNull(secureConfiguration);

        _logger = logger;

        var symmetricSecurityKey = new SymmetricSecurityKey(
            SHA256.HashData(Encoding.UTF8.GetBytes(secureConfiguration.GetAuthenticationKey()))
        );

        _credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
        _validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = secureConfiguration.GetJwtIssuer(),
            ValidAudience = secureConfiguration.GetJwtAudience(),
            IssuerSigningKey = symmetricSecurityKey,
            ClockSkew = TimeSpan.FromMinutes(2)
        };
    }

    public string WriteToken(UserSessionClaims userSessionClaims, bool isPersistent)
    {
        Guard.NotNull(userSessionClaims);

        var expirationTime = UserSessionConfig.GetSessionExpiration(isPersistent);
        var claims = userSessionClaims.GetClaims();

        var token = new JwtSecurityToken(
            issuer: _validationParameters.ValidIssuer,
            audience: _validationParameters.ValidAudience,
            claims: claims,
            expires: DateTime.UtcNow.Add(expirationTime),
            signingCredentials: _credentials
        );

        return _tokenHandler.WriteToken(token);
    }

    public UserSessionClaims? ReadToken(string token)
    {
        var principal = ValidateAndReadPrincipal(token, out var validatedToken);
        if (principal is null || validatedToken is not JwtSecurityToken jwt)
        {
            return null;
        }

        var result = UserSessionClaimsFactory.Create(
            principal.Claims,
            jwt.ValidTo);

        if (result.IsFailure)
        {
            _logger.LogError("Error reading user session token. {Token}. Code: {Code}, Message: {Message} ",
                token,
                result.Error.Code,
                result.Error.Message);
            return null;
        }
        return result.Value;
    }

    private ClaimsPrincipal? ValidateAndReadPrincipal(string authorizationToken, out SecurityToken? validatedToken)
    {
        validatedToken = null;

        if (!_tokenHandler.CanReadToken(authorizationToken))
        {
            _logger.LogError("JwtSecurityTokenHandler.CanReadToken. Invalid token: {AuthorizationToken}. ", authorizationToken);
            return null;
        }

        try
        {
            return _tokenHandler.ValidateToken(authorizationToken, _validationParameters, out validatedToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Invalid token: {AuthorizationToken}. Error : {Message}", authorizationToken, ex.Message);
            return null;
        }
    }
}
