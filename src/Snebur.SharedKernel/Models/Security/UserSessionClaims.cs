namespace Snebur.SharedKernel.Models.Security;

public record UserSessionClaims(
    Guid Session_Id,
    string Name,
    string Email,
    string PhoneNumber,
    bool IsPersistent,
    Language Language,
    UserRole UserRole,
    UserType UserType,
    DateTime? Expiration = null
);
