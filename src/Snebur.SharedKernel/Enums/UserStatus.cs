namespace Snebur.SharedKernel.Enums;

public enum UserStatus
{
    [UndefinedValue]
    Undefined = 0,
    Anonymous = 1,
    New,
    System,
    Online,
    Offline,
    Away,
    Busy,
    DoNotDisturb
}
