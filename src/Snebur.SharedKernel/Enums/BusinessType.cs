using System.ComponentModel;

namespace Snebur.SharedKernel.Enums;

public enum BusinessType
{
    [UndefinedValue]
    [Description("Select Business Type")]
    Undefined = 0,

    [Description("Civil Registry Office")]
    CivilRegistryOffice = 1,

    [SystemValue]
    System = 2
}
