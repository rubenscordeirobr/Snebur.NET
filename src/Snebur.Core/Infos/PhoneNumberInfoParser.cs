using Snebur.Core.Resources;
using Snebur.Core.Utils;

namespace Snebur.Core.Infos;
public static partial class PhoneNumberInfoParser
{
    public static PhoneNumberInfo Parse(
        InternationalDialingCode dialingCode,
        string nationalNumber)
    {
        var numbers = nationalNumber.GetOnlyNumbers();
        if (dialingCode == InternationalDialingCode.Unknown)
        {
            return PhoneNumberInfo.Unknown(numbers);
        }
        return ParseInternal(dialingCode, numbers, removeInternationalCode: false);
    }

    public static PhoneNumberInfo Parse(string? fullNumber)
    {
        if (string.IsNullOrWhiteSpace(fullNumber))
            return PhoneNumberInfo.Unknown(string.Empty);

        var internationalDialingCode = PhoneNumberUtils.GetInternationalDialingCode(fullNumber);

        var numbers = fullNumber.GetOnlyNumbers();
        if (internationalDialingCode == InternationalDialingCode.Unknown)
        {
            return PhoneNumberInfo.Unknown(numbers);
        }
        return ParseInternal(internationalDialingCode, numbers, removeInternationalCode: true);
    }

    private static PhoneNumberInfo ParseInternal(
        InternationalDialingCode internationalDialingCode,
        string numbers,
        bool removeInternationalCode)
    {
        var country = PhoneNumberUtilsInternal.GetCountryCodeInternal(internationalDialingCode);
        var countryMetaDataInfo = PhoneNumberFormatRegistry.TryGet(country);

        if (country == Country.Unknown ||
            internationalDialingCode == InternationalDialingCode.Unknown ||
            countryMetaDataInfo == null)
        {
            return PhoneNumberInfo.Unknown(numbers);
        }

        var nationalNumber = removeInternationalCode
            ? numbers.SafeSubstring(countryMetaDataInfo.InternationalDialingCodeLength)
            : numbers; 

        var codeAreaLength = countryMetaDataInfo.GetBetterAreaCodeLength(nationalNumber.Length);
        var areaCode = nationalNumber.SafeTrim(codeAreaLength);
        var formatterNumber = PhoneNumberUtilsInternal.FormatNatianalNumber(countryMetaDataInfo, nationalNumber);

        return new PhoneNumberInfo
        (
            Country: country,
            InternationalDialingCode: internationalDialingCode,
            NationalNumber: nationalNumber,
            AreaCode: areaCode,
            FormattedNationalNumber: formatterNumber
        );
    }
}

