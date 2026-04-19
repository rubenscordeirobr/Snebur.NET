using System.ComponentModel;

namespace Snebur.Core.Enums;

public enum Culture
{
    [UndefinedValue]
    Undefined = 0,

    // North America
    [Description("en-US")]
    EnUs, // United States

    [Description("en-CA")]
    EnCa, // Canada (English)

    [Description("es-MX")]
    EsMx, // Mexico (Spanish)

    [Description("fr-CA")]
    FrCa, // Canada (French)

    // South America
    [Description("es-AR")]
    EsAr, // Argentina

    [Description("es-BO")]
    EsBo, // Bolivia

    [Description("pt-BR")]
    PtBr, // Brazil

    [Description("es-CL")]
    EsCl, // Chile

    [Description("es-CO")]
    EsCo, // Colombia

    [Description("es-EC")]
    EsEc, // Ecuador

    [Description("en-GY")]
    EnGy, // Guyana (English)

    [Description("es-PE")]
    EsPe, // Peru

    [Description("gn-PY")]
    GnPy, // Paraguay (Guarani and Spanish; using Guarani culture code)

    [Description("nl-SR")]
    NlSr, // Suriname (Dutch)

    [Description("es-UY")]
    EsUy, // Uruguay

    [Description("es-VE")]
    EsVe, // Venezuela

    // Europe
    [Description("es-ES")]
    EsEs, // Spain

    [Description("de-DE")]
    DeDe, // Germany

    [Description("fr-FR")]
    FrFr, // France

    [Description("en-GB")]
    EnGb, // United Kingdom

    [Description("it-IT")]
    ItIt, // Italy

    [Description("pt-PT")]
    PtPt  // Portugal
}
