namespace Snebur.SharedKernel.Enums;

public enum UserState
{
    [UndefinedValue]
    Undefined = 0,
    New = 1,
    Active,
    Inactive ,
    Suspended,
    Deleted,
    PendingVerification,
    Blocked
}
