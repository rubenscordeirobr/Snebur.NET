
namespace Snebur.Core.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public sealed class CountryAbbreviationAttribute : Attribute
{
    public string Abbreviation { get; }

    public CountryAbbreviationAttribute(string abbreviation)
    {
        Abbreviation = abbreviation;
    }
}
