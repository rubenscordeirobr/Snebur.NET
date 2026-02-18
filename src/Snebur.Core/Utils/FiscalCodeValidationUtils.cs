using System.Diagnostics;

namespace Snebur.Core.Utils;

public static class FiscalCodeValidationUtils
{
    public static bool IsValid(string? fiscalCode, Country country)
    {
        if (string.IsNullOrWhiteSpace(fiscalCode))
            return false;

        if (country == Country.Unknown)
            return false;

        if (country == Country.Brazil)
            return IsValidBrazilFiscalCode(fiscalCode);

#if DEBUG
        Trace.TraceError($"FiscalCode validation for {country} is not implemented yet.");
#endif

        return true;

    }

    private static bool IsValidBrazilFiscalCode(string fiscalCode)
    {
        var numbers = fiscalCode.GetOnlyNumbers();
        if (numbers.Length == 11)
        {
            return IsValidBrazilianCPF(numbers);
        }

        if (numbers.Length == 14)
        {
            return IsValidBrazilianCNPJ(numbers);
        }
        return false;
    }

    public static bool IsValidBrazilianCPF(string? cpf)
    {
        if (cpf is null)
            return false;

        if (cpf.Length != 11)
            return false;

        if (cpf.Distinct().Count() == 1)
            return false;

        int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int soma = 0;
        for (int i = 0; i < 9; i++)
            soma += (cpf[i] - '0') * multiplicador1[i];

        int resto = soma % 11;
        int digitoVerificador1 = resto < 2 ? 0 : 11 - resto;
         
        int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        soma = 0;
        for (int i = 0; i < 10; i++)
            soma += (cpf[i] - '0') * multiplicador2[i];

        resto = soma % 11;
        int digitoVerificador2 = resto < 2 ? 0 : 11 - resto;

        return cpf[9] - '0' == digitoVerificador1 && cpf[10] - '0' == digitoVerificador2;
    }

    public static bool IsValidBrazilianCNPJ(string? cnpj)
    {
        if (cnpj is null)
            return false;

        if (cnpj.Length != 14)
            return false;

        if (cnpj.Distinct().Count() == 1)
            return false;

        int[] multiplicador1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int soma = 0;
        for (int i = 0; i < 12; i++)
            soma += (cnpj[i] - '0') * multiplicador1[i];

        int resto = soma % 11;
        int digitoVerificador1 = resto < 2 ? 0 : 11 - resto;

        int[] multiplicador2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        soma = 0;
        for (int i = 0; i < 13; i++)
            soma += (cnpj[i] - '0') * multiplicador2[i];

        resto = soma % 11;
        int digitoVerificador2 = resto < 2 ? 0 : 11 - resto;

        return cnpj[12] - '0' == digitoVerificador1 && cnpj[13] - '0' == digitoVerificador2;
    }
}

