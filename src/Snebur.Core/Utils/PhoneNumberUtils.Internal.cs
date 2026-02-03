using System.ComponentModel;
using Snebur.Core.Infos;

namespace Snebur.Core.Utils;

internal static partial class PhoneNumberUtilsInternal
{
    private static readonly HashSet<string> _canadianAreaCodes = new()
        {
          "204", "226", "236", "249", "250", "289", "306", "343", "365", "387",
          "403", "416", "418", "431", "437", "438", "450", "506", "514", "519",
          "548", "579", "581", "587", "600", "604", "613", "639", "647", "672",
          "705", "709", "742", "778", "780", "782", "807", "819", "825", "867",
          "873", "902", "905"
        };

    private static readonly Dictionary<InternationalDialingCode, Country> _mappings = new() {
            { InternationalDialingCode.Unknown, Country.Unknown },
            { InternationalDialingCode.UnitedStates, Country.UnitedStates },
            { InternationalDialingCode.Canada, Country.Canada },
            { InternationalDialingCode.Mexico, Country.Mexico },
            { InternationalDialingCode.Argentina, Country.Argentina },
            { InternationalDialingCode.Bolivia, Country.Bolivia },
            { InternationalDialingCode.Brazil, Country.Brazil },
            { InternationalDialingCode.Chile, Country.Chile },
            { InternationalDialingCode.Colombia, Country.Colombia },
            { InternationalDialingCode.Ecuador, Country.Ecuador },
            { InternationalDialingCode.Guyana, Country.Guyana },
            { InternationalDialingCode.Paraguay, Country.Portugal },
            { InternationalDialingCode.Peru, Country.Peru },
            { InternationalDialingCode.Suriname, Country.Suriname },
            { InternationalDialingCode.Uruguay, Country.Uruguay },
            { InternationalDialingCode.Venezuela, Country.Venezuela },
            { InternationalDialingCode.Italy, Country.Italy },
            { InternationalDialingCode.France, Country.France },
            { InternationalDialingCode.Germany, Country.Germany },
            { InternationalDialingCode.Spain, Country.Spain },
            { InternationalDialingCode.Portugal, Country.Portugal },
            { InternationalDialingCode.UnitedKingdom, Country.UnitedKingdom }
        };

    internal static string FormatNatianalNumber(
        PhoneNumberFormatInfo? countryMetedataInfo,
        string nationalNumber)
    {
        if (countryMetedataInfo == null)
        {
            return nationalNumber;
        }

        if (countryMetedataInfo.IsLeadingZeroNeeded)
        {
            nationalNumber = $"0{nationalNumber}";
        }

        var format = countryMetedataInfo.GetBetterNationalFormat(nationalNumber.Length);
        var provider = new MaskedTextProvider(format, true);
        var isSucess = provider.Set(nationalNumber);
        if (!isSucess)
        {
            return nationalNumber;
        }
        return provider.ToDisplayString();
    }

    internal static Country GetCountryCodeInternal(
        InternationalDialingCode internationalDialingCode )
    {
        return _mappings.TryGetValue(internationalDialingCode, out var countryCode)
             ? countryCode
             : Country.Unknown;
    }

    internal static InternationalDialingCode GetInternationalDialingCodeInternal(string fullNumber)
    {
        if (fullNumber is null || !fullNumber.TrimStart().StartsWith('+'))
            return InternationalDialingCode.Unknown;

        var numbers = fullNumber.GetOnlyNumbers();
        if (numbers.Length > 2)
        {
            if (EnumUtils.TryParse<InternationalDialingCode>(numbers[..3], out var result))
            {
                return result;
            }
        }
        if (numbers.Length > 1)
        {
            if (EnumUtils.TryParse<InternationalDialingCode>(numbers[..2], out var result2))
            {
                return result2;
            }
        }
       
        if (numbers.StartsWith('1'))
        {
            var areaCode = numbers[1..3];
            if (_canadianAreaCodes.Contains(areaCode))
            {
                return InternationalDialingCode.Canada;
            }
            return InternationalDialingCode.UnitedStates;
        }
        return InternationalDialingCode.Unknown;
    }
}
