namespace Snebur.SharedKernel.Enums;

public enum TenantStatus
{
    [UndefinedValue]
    Unknown = 0,
    Active,
    Inactive,
    Suspended,
    Pending,
    Archived
}
