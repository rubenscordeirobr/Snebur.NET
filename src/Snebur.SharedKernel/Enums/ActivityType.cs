namespace Snebur.SharedKernel.Enums;

public enum ActivityType
{
    [UndefinedValue]
    Unknown = 0,
    Created,
    Updated,
    Deleted,
    Read,
    Authenticated,
    UserLoginSuccess,
    UserLoginFailed,
    UserLogout,
}
