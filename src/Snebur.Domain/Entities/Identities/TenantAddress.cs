namespace Snebur.Domain.Entities.Identities;

public sealed class TenantAddress : EntityBase, ITenantOwned, IAddress, ISoftDeletableEntity, IAscendingSortable
{
    public string AddressName { get; private set; }
    public string Street { get; private set; }
    public string Number { get; private set; }
    public string? Complement { get; private set; }
    public string Neighborhood { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public string ZipCode { get; private set; }
    public Country Country { get; private set; }
    public Guid Tenant_Id { get; private set; }
    public Tenant? Tenant { get; private set; }
    public bool IsDefault { get; private set; }

    internal TenantAddress(
           Tenant tenant,
           string addressName,
           string street,
           string number,
           string? complement,
           string neighborhood,
           string city,
           string state,
           string zipCode,
           Country country)
           : this(addressName, street, number, complement, neighborhood,
                 city, state, zipCode, country, tenant.Id)
    {
        Tenant = tenant;
    }

    // EF Core
    private TenantAddress(
        string addressName,
        string street,
        string number,
        string? complement,
        string neighborhood,
        string city,
        string state,
        string zipCode,
        Country country,
        Guid tenant_Id)
    {
        AddressName = addressName;
        Street = street;
        Number = number;
        Complement = complement;
        Neighborhood = neighborhood;
        City = city;
        State = state;
        Country = country;
        ZipCode = zipCode;
        Tenant_Id = tenant_Id;
    }

    public void SetAddressName(string addressName)
    {
        AddressName = addressName;
    }

    internal void RemoveDefault()
    {
        IsDefault = false;
    }

    internal void SetDefault()
    {
        IsDefault = true;
    }

    #region IEntityDeleted, IOrderableEntity

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public Guid? DeletedSession_Id { get; private set; }
    public double? SortOrder { get; private set; }

    #endregion
}
