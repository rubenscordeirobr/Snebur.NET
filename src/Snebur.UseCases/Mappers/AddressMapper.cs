using Snebur.UseCases.Shared;

namespace Snebur.UseCases.Mappers;

public static class AddressMapper
{
    internal static AddressDto? ToDto(TenantAddress? defaultAddress)
    {
        if (defaultAddress is null)
        {
            return null;
        }

        return new AddressDto
        {
            AddressName = defaultAddress.AddressName,
            Street = defaultAddress.Street,
            Number = defaultAddress.Number,
            Complement = defaultAddress.Complement,
            Neighborhood = defaultAddress.Neighborhood,
            City = defaultAddress.City,
            State = defaultAddress.State,
            ZipCode = defaultAddress.ZipCode,
            Country = defaultAddress.Country
        };
    }
}
