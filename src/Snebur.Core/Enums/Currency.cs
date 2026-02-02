using System.ComponentModel;

namespace Snebur.Core.Enums;

public enum Currency
{
    [UndefinedValue]
    Unknown = 0,

    [Description("US Dollar (USD)")]
    USD = 840,

    [Description("Brazilian Real (BRL)")]
    BRL = 986,

    [Description("Euro (EUR)")]
    EUR = 978
}
