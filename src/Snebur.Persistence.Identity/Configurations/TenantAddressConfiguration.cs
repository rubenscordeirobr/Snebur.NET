namespace Snebur.Persistence.Identity.Configurations;

public class TenantAddressConfiguration : IEntityTypeConfiguration<TenantAddress>
{
    public void Configure(EntityTypeBuilder<TenantAddress> builder)
    {
        Guard.NotNull(builder);

        builder.Property(x => x.AddressName)
            .IsRequired()
            .HasMaxLength(ValidationConstants.AddressNameMaxLength);

        builder.Property(x => x.Street)
            .IsRequired()
            .HasMaxLength(ValidationConstants.StreetMaxLength);

        builder.Property(x => x.Number)
            .IsRequired()
            .HasMaxLength(ValidationConstants.AddressNumberMaxLength);

        builder.Property(x => x.Complement)
            .IsRequired(false)
            .HasMaxLength(ValidationConstants.AddressComplementMaxLength);

        builder.Property(x => x.Neighborhood)
            .IsRequired()
            .HasMaxLength(ValidationConstants.NeighborhoodMaxLength);

        builder.Property(x => x.City)
            .IsRequired()
            .HasMaxLength(ValidationConstants.CityMaxLength);

        builder.Property(x => x.State)
            .IsRequired()
            .HasMaxLength(ValidationConstants.AddressStateMaxLength);
  
        builder.Property(x => x.ZipCode)

            .IsRequired()
            .HasMaxLength(ValidationConstants.ZipCodeMaxLength);
    }
}
