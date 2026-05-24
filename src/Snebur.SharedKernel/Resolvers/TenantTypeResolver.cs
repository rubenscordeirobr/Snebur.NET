using Snebur.Core.Utils;

namespace Snebur.SharedKernel.Resolvers;
public static class TenantTypeResolver
{
    public static TenantType GetTenantType(Country country, string fiscalCodeString)
    {
        if(country == Country.Brazil)
        {
            if(FiscalCodeValidationUtils.IsValidBrazilianCPF(fiscalCodeString))
            {
                return TenantType.Individual;
            }
        }
        return TenantType.Company;
    }
}
