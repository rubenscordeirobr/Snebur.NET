using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Snebur.Core.Enums;

public enum Country
{
    [UndefinedValue]
    Unknown = 0,

    [Description("United States")]
    [CountryNumericCode(840)]
    [CountryAbbreviation("USA")]

    UnitedStates,

    [Description("Canada")]
    [CountryNumericCode(124)]
    [CountryAbbreviation("CAN")]
    Canada,

    [Description("Mexico")]
    [CountryNumericCode(484)]
    [CountryAbbreviation("MEX")]
    Mexico,

    // South America
    [Description("Argentina")]
    [CountryNumericCode(32)]
    [CountryAbbreviation("ARG")]
    Argentina,

    [Description("Bolivia")]
    [CountryNumericCode(68)]
    [CountryAbbreviation("BOL")]
    Bolivia,

    [Description("Brazil")]
    [CountryNumericCode(76)]
    [CountryAbbreviation("BRA")]
    Brazil,

    [Display(Name = "Chile")]
    [CountryNumericCode(152)]
    [CountryAbbreviation("CHL")]
    Chile,

    [Description("Colombia")]
    [CountryNumericCode(170)]
    [CountryAbbreviation("COL")]
    Colombia,

    [Description("Ecuador")]
    [CountryNumericCode(218)]
    [CountryAbbreviation("ECU")]
    Ecuador,

    [Description("Guyana")]
    [CountryNumericCode(328)]
    [CountryAbbreviation("GUY")]
    Guyana,

    [Description("Peru")]
    [CountryNumericCode(604)]
    [CountryAbbreviation("PER")]
    Peru,

    [Description("Paraguay")]
    [CountryNumericCode(600)]
    [CountryAbbreviation("PRY")]
    Paraguay,

    [Description("Suriname")]
    [CountryNumericCode(740)]
    [CountryAbbreviation("SUR")]
    Suriname,

    [Description("Uruguay")]
    [CountryNumericCode(858)]
    [CountryAbbreviation("URY")]
    Uruguay,

    [Description("Venezuela")]
    [CountryNumericCode(862)]
    [CountryAbbreviation("VEN")]
    Venezuela,

    // Europe
    [Description("Spain")]
    [CountryNumericCode(724)]
    [CountryAbbreviation("ESP")]
    Spain,

    [Description("Germany")]
    [CountryNumericCode(276)]
    [CountryAbbreviation("DEU")]
    Germany,

    [Description("France")]
    [CountryNumericCode(250)]
    [CountryAbbreviation("FRA")]
    France,

    [Description("United Kingdom")]
    [CountryNumericCode(826)]
    [CountryAbbreviation("GBR")]
    UnitedKingdom,

    [Description("Italy")]
    [CountryNumericCode(380)]
    [CountryAbbreviation("ITA")]
    Italy,

    [Description("Portugal")]
    [CountryNumericCode(620)]
    [CountryAbbreviation("PRT")]
    Portugal,
}
