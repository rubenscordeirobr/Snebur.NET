using Snebur.Core.Infos;
using Snebur.Core.Resources;

namespace Snebur.Core.Utils;

public static class PhoneNumberValidationUtils
{
    public static bool IsFullPhoneNumberValid(string? fullPhoneNumber)
    {
        if (fullPhoneNumber == null)
        {
            return false;
        }

        var country = PhoneNumberUtils.GetCountryCode(fullPhoneNumber);
        var phoneFormatInfo = PhoneNumberFormatRegistry.TryGet(country);
        if (phoneFormatInfo == null)
        {
            return false;
        }

        var numbers = fullPhoneNumber.GetOnlyNumbers();
        var nationalNumber = numbers.SafeSubstring(phoneFormatInfo.InternationalDialingCodeLength);
        return IsNationalNumberValid(phoneFormatInfo, nationalNumber);
    }

    public static bool IsNationalNumberValid(
        Country countryCode,
        string? nationalNumber)
    {
        if (countryCode == Country.Unknown)
            return false;

        if (string.IsNullOrEmpty(nationalNumber))
            return false;

        var phoneFormatInfo = PhoneNumberFormatRegistry.TryGet(countryCode);
        if (phoneFormatInfo == null)
        {
            return false;
        }
        return IsNationalNumberValid(phoneFormatInfo, nationalNumber);
    }

    public static bool IsValidAreaCode(Country country, string? areaCode)
    {
        if (string.IsNullOrWhiteSpace(areaCode))
            return false;

        if (country == Country.Brazil)
        {
            return IsBrazilianAreaCodeValid(areaCode);
        }
        return true;
    }

    private static bool IsNationalNumberValid(
        PhoneNumberFormatInfo phoneFormatInfo,
        string nationalNumber)
    {
        var minLength = phoneFormatInfo.MinNationalNumberLength;
        var maxLength = phoneFormatInfo.MaxNationalNumberLength;
        return nationalNumber.Length >= minLength
            && nationalNumber.Length <= maxLength;
    }

    private static bool IsBrazilianAreaCodeValid(string areaCode)
    {
        if (string.IsNullOrWhiteSpace(areaCode))
            return false;

        var areaCodeInt = int.Parse(areaCode);
        if (areaCodeInt < 11 || areaCodeInt > 99)
            return false;

        if (!EnumUtils.IsDefined((BrazilianAreaCode)areaCodeInt))
            return false;

        return true;
    }

}
