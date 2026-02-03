
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Snebur.Core.Utils;

public static class ValidationUtils
{
    private static readonly Regex _hexRegex = new(@"^[a-fA-F0-9]+$", RegexOptions.Compiled);

    private static readonly Regex _emailRegex = new(
       @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|([-!#$%&'*+/=?^_`{|}~\w]|\.(?!\.))+)(?<=\S)@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$",
       RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);

    public static bool IsEmail([NotNullWhen(true)] string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return true;

        if (value.Length < 3 || value.Length > 254)
            return false;

        return _emailRegex.IsMatch(value);

    }
    public static bool IsNationalNumberValid(Country country, [NotNullWhen(true)] string? phoneNumber)
    {
        return PhoneNumberValidationUtils.IsNationalNumberValid(country, phoneNumber);
    }

    public static bool IsFullPhoneNumberValid([NotNullWhen(true)] string? phoneNumber)
    {
        return PhoneNumberValidationUtils.IsFullPhoneNumberValid(phoneNumber);
    }

    public static bool IsSha256(string? value)
    {
        if (value == null)
            return false;

        if (value.Length != 64)
            return false;

        return IsHex(value);
    }

    public static bool IsSha1(string? value)
    {
        if (value == null)
            return false;

        if (value.Length != 40)
            return false;

        return IsHex(value);
    }

    public static bool IsMd5(string? value)
    {
        if (value == null)
            return false;

        if (value.Length != 32)
            return false;

        return IsHex(value);
    }

    public static bool IsHex(string value)
    {
        return _hexRegex.IsMatch(value);
    }

    public static bool IsGuid(string? value)
    {
        if (value == null)
            return false;
        return Guid.TryParse(value, out _);
    }

 
}
