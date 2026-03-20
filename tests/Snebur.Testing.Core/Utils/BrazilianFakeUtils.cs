using Snebur.Core.Utils;

namespace Snebur.Testing.Core.Utils;

public static class BrazilianFakeUtils
{
    private static readonly Random _random = new Random();

    public static string GenerateCpf(bool formatted = true)
    {
        var cpf = new int[11];

        // Generate the first 9 digits randomly
        for (var i = 0; i < 9; i++)
        {
#pragma warning disable CA5394  
            cpf[i] = _random.Next(0, 10);
#pragma warning restore CA5394
        }

        // Calculate the first verification digit
        var sum = 0;
        for (var i = 0; i < 9; i++)
        {
            sum += cpf[i] * (10 - i);
        }
        var remainder = sum * 10 % 11;
        cpf[9] = remainder == 10 || remainder == 11 ? 0 : remainder;

        // Calculate the second verification digit
        sum = 0;
        for (var i = 0; i < 10; i++)
        {
            sum += cpf[i] * (11 - i);
        }
        remainder = sum * 10 % 11;
        cpf[10] = remainder == 10 || remainder == 11 ? 0 : remainder;

        var concatedCpf = string.Concat(cpf);
        if (formatted)
        {
            return BrazilianFormattingUtils.FormatCpf(concatedCpf);
        }
        return concatedCpf;

    }

    public static string GenerateFakeCnpj(bool formatted = true)
    {
        var cnpj = new int[14];

        // Generate the first 12 digits randomly
        for (var i = 0; i < 12; i++)
        {
#pragma warning disable CA5394
            cnpj[i] = _random.Next(0, 10);
#pragma warning restore CA5394
        }

        // Calculate the first verification digit
        var sum = 0;
        var weight = new int[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        for (var i = 0; i < 12; i++)
        {
            sum += cnpj[i] * weight[i];
        }
        var remainder = sum % 11;
        cnpj[12] = remainder < 2 ? 0 : 11 - remainder;

        // Calculate the second verification digit
        sum = 0;
        weight = new int[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        for (var i = 0; i < 13; i++)
        {
            sum += cnpj[i] * weight[i];
        }
        remainder = sum % 11;
        cnpj[13] = remainder < 2 ? 0 : 11 - remainder;

        var concatedCnpj = string.Concat(cnpj);
        if (formatted)
        {
            return BrazilianFormattingUtils.FormatCnpj(concatedCnpj);
        }
        return concatedCnpj;
    }

    public static string GenerateFakePhoneNumber()
    {
        return $"+55 11 9{RandomUtils.GenerateRandomNumber(4)}-{RandomUtils.GenerateRandomNumber(4)}";
    }

}
