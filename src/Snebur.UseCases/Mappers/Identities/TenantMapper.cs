namespace Snebur.UseCases.Mappers.Identities;

public static class TenantMapper
{
    public static TenantResponse ToResponse(Tenant tenant)
    {
        Guard.NotNull(tenant);

        var addressDto = AddressMapper.ToDto(tenant.DefaultAddress);

        return new TenantResponse
        {
            Id = tenant.Id,
            Name = tenant.Name,
            Email = tenant.Email,
            PhoneNumber = tenant.PhoneNumber,
            Address = addressDto,
            TenantState = tenant.TenantState,
            TenantStatus = tenant.TenantStatus,
            BusinessType = tenant.BusinessType,
            Country = tenant.Country,
            Currency = tenant.Currency,
            FiscalCode = tenant.FiscalCode,
            Language = tenant.Language,
            TenantName = tenant.Name,
            TenantType = tenant.TenantType,
            TimeZoneOffset = tenant.TimeZoneOffset,
        };
    }
}
