namespace Snebur.Core.Infos;

public partial record PhoneNumberFormatInfo
{
    public Country CountryCode { get; init; }
    public InternationalDialingCode InternationalDialingCode { get; init; }
    public int MinAreaCodeLength { get; init; }
    public int MaxAreaCodeLength { get; init; }
    public int MinNationalNumberLength { get; init; }
    public int MaxNationalNumberLength { get; init; }
    public bool IsLeadingZeroNeeded { get; init; }
    public string? TrunkPrefix { get; init; }
    //Ex Brazil (##) #####-####
    public required string PrimaryNationalFormat { get; init; }
    public required int InternationalDialingCodeLength { get; init; }

    public (string Format, int NationalLength)[]? AlternateNationalFormats { get; init; }

    public string GetBetterNationalFormat(int nationalNumberLength)
    {
        if (AlternateNationalFormats == null || AlternateNationalFormats.Length == 0)
        {
            return PrimaryNationalFormat;
        }

        foreach (var (format, length) in AlternateNationalFormats)
        {
            if (length == nationalNumberLength)
            {
                return format;
            }
        }
        return PrimaryNationalFormat;
    }

    internal int GetBetterAreaCodeLength(int nationalNumberLength)
    {
        if (MinAreaCodeLength == MaxAreaCodeLength)
            return MinAreaCodeLength;

        if(MinNationalNumberLength == nationalNumberLength)
            return MinAreaCodeLength;

        if (MaxNationalNumberLength == nationalNumberLength)
            return MaxAreaCodeLength;

        var diff = nationalNumberLength - MinNationalNumberLength;
        return MaxAreaCodeLength - diff;
    }
}

