using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Snebur.Core.Enums;

public enum Language
{
    [SystemValue]
    [Description("Default")]
    Default,

    [Description("Portuguese (Brazil)")]
    PortugueseBrazil,

    [Description("Portuguese (Portugal)")]
    PortuguesePortugal,

    [Description("English (Standard)")]
    English,

    [Description("Spanish (Latin America)")]
    LatinSpanish,

    [Description(   "Spanish (Spain)")]
    Spanish,

    [Description("French")]
    French,

    [Description("German")]
    German,

    [Description("Italian")]
    Italian,
}
