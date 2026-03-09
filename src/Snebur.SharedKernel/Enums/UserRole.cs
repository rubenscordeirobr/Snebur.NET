namespace Snebur.SharedKernel.Enums;

public enum UserRole
{
    [UndefinedValue]
    Undefined = 0,
    Anonymous,
    SystemAdmin,
    Owner,
    Admin,
    Operator,
    Viewer,
    ChatAgent,
}
