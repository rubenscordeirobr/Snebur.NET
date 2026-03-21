using System.Globalization;

namespace Snebur.Core.Utils;

public static class BrazilianFormattingUtils
{
    public static string FormatFiscalCode(string fiscalCode)
    {
        var numbers = fiscalCode.GetOnlyNumbers();
        if (numbers.Length == 11)
        {
            return FormatCpf(numbers);
        }
        else if (numbers.Length == 14)
        {
            return FormatCnpj(numbers);
        }
        return fiscalCode;
    }

    public static string FormatCpf(string cpf)
    {
        var numbers = cpf.GetOnlyNumbers();
        if (numbers.Length != 11)
        {
            return cpf;
        }
        return $"{numbers[..3]}.{numbers[3..6]}.{numbers[6..9]}-{numbers[9..]}";
    }

    public static string FormatCnpj(string cnpj)
    {
        var numbers = cnpj.GetOnlyNumbers();
        if (numbers.Length != 14)
        {
            return cnpj;
        }
        return $"{numbers[..2]}.{numbers[2..5]}.{numbers[5..8]}/{numbers[8..12]}-{numbers[12..]}";
    }

    public static string FormatCep(string cep)
    {
        var numbers = cep.GetOnlyNumbers();
        if (numbers.Length != 8)
        {
            return cep;
        }
        return $"{numbers[..5]}-{numbers[5..]}";
    }

    public static string FormatPhone(
        string phone,
        bool internationalFormat)
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            return phone;
        }

        var numbers = phone.GetOnlyNumbers();
        if (numbers.StartsWith("+55"))
        {
            numbers = numbers[3..];
        }
        else if (numbers.StartsWith("55"))
        {
            numbers = numbers[2..];
        }
        else if (phone.StartsWith('0'))
        {
            numbers = phone[1..];
        }

        if (numbers.Length < 10 || numbers.Length > 11)
            return phone;

        var formatted = numbers.Length == 10
            ? $"({numbers[..2]}) {numbers[2..6]}-{numbers[6..]}"
            : $"({numbers[..2]}) {numbers[2..7]}-{numbers[7..]}";

        return internationalFormat
            ? $"+55 {formatted}"
            : formatted;
    }

    public static string FormatMoney(decimal value)
    {
        var isNegative = value < 0;
        value = Math.Abs(value);

        var intPart = (int)value;
        var decimalPart = (int)((value - intPart) * 100);

        var intPartFormatted = string.Format(CultureInfo.InvariantCulture, "{0:N0}", intPart)
             .Replace(",", ".");

        var decimalFormatted = decimalPart.ToString("D2");

        //\u00A0 no break space
        var formatted = $"R$\u00A0{intPartFormatted},{decimalFormatted}";
        return isNegative ? $"-{formatted}" : formatted;
    }
}

