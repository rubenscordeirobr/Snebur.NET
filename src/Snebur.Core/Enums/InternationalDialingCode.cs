using System.ComponentModel;

namespace Snebur.Core.Enums;

public enum InternationalDialingCode
{
    [UndefinedValue]
    [Description("Unknown")]
    Unknown = 0,

    // North America
    [SharedInternationalDialingCode]
    [Description("United States (+1)")]
    [DialingCode("+1")]
    UnitedStates = 1,

    [SharedInternationalDialingCode]
    [Description("Canada (+1)")]
    [DialingCode("+1")]
    Canada = 2,

    [Description("Mexico (+52)")]
    [DialingCode("+52")]
    Mexico = 52,

    // South America
    [Description("Argentina (+54)")]
    [DialingCode("+54")]
    Argentina = 54,

    [Description("Bolivia (+591)")]
    [DialingCode("+591")]
    Bolivia = 591,

    [Description("Brazil (+55)")]
    [DialingCode("+55")]
    Brazil = 55,

    [Description("Chile (+56)")]
    [DialingCode("+56")]
    Chile = 56,

    [Description("Colombia (+57)")]
    [DialingCode("+57")]
    Colombia = 57,

    [Description("Ecuador (+593)")]
    [DialingCode("+593")]
    Ecuador = 593,

    [Description("Guyana (+592)")]
    Guyana = 592,

    [Description("Paraguay (+595)")]
    [DialingCode("+595")]
    Paraguay = 595,

    [Description("Peru (+51)")]
    [DialingCode("+51")]
    Peru = 51,

    [Description("Suriname (+597)")]
    [DialingCode("+597")]
    Suriname = 597,

    [Description("Uruguay (+598)")]
    [DialingCode("+598")]
    Uruguay = 598,

    [Description("Venezuela (+58)")]
    [DialingCode("+58")]
    Venezuela = 58,

    // Europe
    [Description("Italy (+39)")]
    [DialingCode("+39")]
    Italy = 39,

    [Description("France (+33)")]
    [DialingCode("+33")]
    France = 33,

    [Description("Germany (+49)")]
    [DialingCode("+49")]
    Germany = 49,

    [Description("Spain (+34)")]
    [DialingCode("+34")]
    Spain = 34,

    [Description("Portugal (+351)")]
    [DialingCode("+351")]
    Portugal = 351,

    [Description("UK (+44)")]
    [DialingCode("+44")]
    UnitedKingdom = 44
}
