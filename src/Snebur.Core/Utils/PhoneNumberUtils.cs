using Snebur.Core.Resources;

namespace Snebur.Core.Utils;

public static partial class PhoneNumberUtils
{
    public static Country GetCountryCode(string fullNumber)
    {
        var internationalDialingCode = GetInternationalDialingCode(fullNumber);
        return PhoneNumberUtilsInternal.GetCountryCodeInternal(
            internationalDialingCode);
    }

    public static string GetFullPhoneNumber(Country country, string nationalNumber)
    {
        var phoneFormatInfo = PhoneNumberFormatRegistry.TryGet(country);
        if (phoneFormatInfo == null)
        {
            throw new ArgumentException(
                $"Country '{country}' is not supported for phone number formatting.", nameof(country));
        }

        var cleanedNumber = nationalNumber.GetOnlyNumbers('+');
        if (cleanedNumber.Length < phoneFormatInfo.MinNationalNumberLength)
        {
            throw new ArgumentException(
                $"National number '{nationalNumber}' is too short. Minimum length required: {phoneFormatInfo.MinNationalNumberLength}.", nameof(nationalNumber));
        }

        // Check if the number already includes international format with "+"
        if (cleanedNumber.StartsWith('+'))
        {
            var providedDialingCode = cleanedNumber.SafeSubstring(0, phoneFormatInfo.InternationalDialingCodeLength + 1);
            var expectedDialingCode = phoneFormatInfo.InternationalDialingCode.GetDialingCode();

            if (providedDialingCode != expectedDialingCode)
            {
                throw new ArgumentException(
                $"National number '{nationalNumber}' contains invalid international dialing code. Expected: +{expectedDialingCode}.", nameof(nationalNumber));
            }
            return cleanedNumber;
        }

        var dialingCode = phoneFormatInfo.InternationalDialingCode.GetDialingCode();
        return $"{dialingCode}{cleanedNumber}";
    }

    public static InternationalDialingCode GetInternationalDialingCode(string fullNumber)
    {
        return PhoneNumberUtilsInternal.GetInternationalDialingCodeInternal(fullNumber);
    }

    public static InternationalDialingCode GetInternationalDialingCodeFromCountry(Country country)
    {
        var phoneFormatInfo = PhoneNumberFormatRegistry.TryGet(country);
        if (phoneFormatInfo == null)
        {
            return InternationalDialingCode.Unknown;
        }
        return phoneFormatInfo.InternationalDialingCode;
    }
}

