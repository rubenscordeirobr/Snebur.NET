using System.Security.Claims;
using Snebur.Core.Utils;
using Snebur.SharedKernel.Constants;
using Snebur.SharedKernel.Interfaces.Identities;
using Snebur.SharedKernel.Models.Security;

namespace Snebur.SharedKernel.Factories;

public static class UserSessionClaimsFactory
{
    public static UserSessionClaims Create(
        IUserSession userSession,
        IUser user)
    {
        Guard.NotNull(userSession);
        Guard.NotNull(user);

        return CreateInternal(
            userSession.Id,
            user.Name,
            user.Email,
            user.PhoneNumber.FullNumber,
            userSession.IsPersistent,
            userSession.Language,
            userSession.UserRole,
            userSession.UserType,
            null);
    }

    public static Result<UserSessionClaims> Create(
        IEnumerable<Claim> claims, 
        DateTime expiration)
    {
        Guard.NotNull(claims);

        var claimsList = claims as IList<Claim> ?? [.. claims];
        
        var sessionId = claimsList.FirstOrDefault(c => c.Type == UserSessionClaimTypes.SessionId)?.Value;
        var name = claimsList.FirstOrDefault(static c => c.Type == ClaimTypes.Name)?.Value;
        var email = claimsList.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var phoneNumber = claimsList.FirstOrDefault(c => c.Type == ClaimTypes.MobilePhone)?.Value;
        var isPersistent = claimsList.FirstOrDefault(c => c.Type == ClaimTypes.IsPersistent)?.Value;
        var language = claimsList.FirstOrDefault(c => c.Type == UserSessionClaimTypes.Language)?.Value;
        var userRole = claimsList.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        var userType = claimsList.FirstOrDefault(c => c.Type == UserSessionClaimTypes.UserType)?.Value;
         
        return Create(
           sessionIdString: sessionId,
           name: name,
           email: email,
           phoneNumber: phoneNumber,
           isPersistentString: isPersistent,
           languageString: language,
           userRoleString: userRole,
           userTypeString: userType,
           expiration: expiration);
    }

    private static Result<UserSessionClaims> Create(
        string? sessionIdString,
        string? name,
        string? email,
        string? phoneNumber,
        string? isPersistentString,
        string? languageString,
        string? userRoleString,
        string? userTypeString,
        DateTime? expiration)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<UserSessionClaims>(
                new ParserError(
                    null,
                    "UserSessionClaims.Create",
                    "Name is null or empty"));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            return Result.Failure<UserSessionClaims>(
                new ParserError(
                    null,
                    "UserSessionClaims.Create",
                    "Email is null or empty"));
        }

        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return Result.Failure<UserSessionClaims>(
                new ParserError(
                    null,
                    "UserSessionClaims.Create",
                    "PhoneNumber is null or empty"));
        }

        if (string.IsNullOrWhiteSpace(isPersistentString))
        {
            return Result.Failure<UserSessionClaims>(
                new ParserError(
                    null,
                    "UserSessionClaims.Create",
                    "IsPersistent is null or empty"));
        }

        if (!bool.TryParse(isPersistentString, out var isPersistent))
        {
            return Result.Failure<UserSessionClaims>(
                new ParserError(
                    null,
                    "UserSessionClaims.Create",
                    $"IsPersistent: Failed to parse bool from string: {isPersistentString}"));
        }

        if (!Guid.TryParse(sessionIdString, out var session_Id))
        {
            return Result.Failure<UserSessionClaims>(
                new ParserError(
                    null,
                    "UserSessionClaims.Create",
                    $"Failed to parse Guid from string: {sessionIdString}"));
        }

        if (!EnumUtils.TryParse<Language>(languageString, out var language))
        {
            return Result.Failure<UserSessionClaims>(
                new ParserError(
                    null,
                    "UserSessionClaims.Create",
                    $"Failed to parse Language from string: {languageString}"));
        }

        if (!EnumUtils.TryParse<UserType>(userTypeString, out var userType))
        {
            return Result.Failure<UserSessionClaims>(
                new ParserError(
                    null,
                    "UserSessionClaims.Create",
                    $"Failed to parse UserType from string: {userTypeString}"));
        }

        if (!EnumUtils.TryParse<UserRole>(userRoleString, out var userRole))
        {
            return Result.Failure<UserSessionClaims>(
                new ParserError(
                    null,
                    "UserSessionClaims.Create",
                    $"Failed to parse UserRole from string: {userRoleString}"));
        }

        if (expiration is null)
        {
            return Result.Failure<UserSessionClaims>(
                new ParserError(
                    null,
                    "UserSessionClaims.Create",
                    $"Expiration is null"));
        }

        if (expiration < DateTime.UtcNow)
        {
            return Result.Failure<UserSessionClaims>(
                new ParserError(
                    null,
                    "UserSessionClaims.Create",
                    $"Expiration is in the past"));
        }

        var userSessionClaims = CreateInternal(session_Id,
            name,
            email,
            phoneNumber,
            isPersistent,
            language,
            userRole,
            userType,
            expiration);

        return Result.Success(userSessionClaims);

    }

    private static UserSessionClaims CreateInternal(
        Guid session_Id,
        string name,
        string email,
        string phoneNumber,
        bool isPersistent,
        Language language,
        UserRole userRole,
        UserType userType,
        DateTime? expiration)
    {
        return new UserSessionClaims(
            session_Id,
            name,
            email,
            phoneNumber,
            isPersistent,
            language,
            userRole,
            userType,
            expiration);
    }
}
