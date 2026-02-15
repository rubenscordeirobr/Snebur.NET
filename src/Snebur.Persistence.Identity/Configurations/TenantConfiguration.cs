namespace Snebur.Persistence.Identity.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        Guard.NotNull(builder);

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(ValidationConstants.NameMaxLength);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(ValidationConstants.EmailMaxLength);

        builder.Property(x => x.FiscalCode)
            .IsRequired()
            .HasMaxLength(ValidationConstants.FiscalCodeMaxLength);
  
        builder.HasMany(x => x.Addresses)
          .WithOne(x => x.Tenant)
          .HasForeignKey(x => x.Tenant_Id)
          .IsRequired();

        builder.HasMany(x => x.Sessions)
          .WithOne(x => x.Tenant)
          .HasForeignKey(x => x.Tenant_Id)
          .IsRequired();

        builder.HasOne(x => x.DefaultAddress)
            .WithOne()
            .HasForeignKey<Tenant>(x => x.Address_Id)
            .IsRequired(false);

        builder.HasOne(x => x.OwnerUser)
            .WithOne()
            .HasForeignKey<Tenant>(x => x.OwnerUser_Id)
            .IsRequired(false);

        builder.HasMany(x => x.Users)
            .WithOne(x => x.Tenant)
            .HasForeignKey(x => x.Tenant_Id)
            .IsRequired();
         
        builder.HasIndex(x => x.FiscalCode)
            .IsUnique();

        builder.HasIndex(x => x.Email)
            .IsUnique();
    }
}
