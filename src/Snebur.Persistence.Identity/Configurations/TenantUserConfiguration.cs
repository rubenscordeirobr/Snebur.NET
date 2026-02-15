namespace Snebur.Persistence.Identity.Configurations;

public class TenantUserConfiguration : IEntityTypeConfiguration<TenantUser>
{
    public void Configure(EntityTypeBuilder<TenantUser> builder)
    {
        Guard.NotNull(builder);

        builder.HasBaseType<User>();

        builder.HasOne(x => x.Tenant)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.Tenant_Id)
            .IsRequired();

        var metadata = builder.Metadata;

        // If the "Discriminator" property exists, it indicates an EF Core relational database
        // otherwise, it is an EF Core in-memory database.
        var isExitsDiscriminator = metadata.GetProperties().Any(x => x.Name == "Discriminator");
        if (!isExitsDiscriminator)
        {
            return;
        }

        string[] emailUniqueIndexProperties = [
            nameof(User.Email),
            nameof(TenantUser.Tenant_Id),
            "Discriminator"
        ];

        string[] phoneNumberUniqueIndexProperties = [
            nameof(User.PhoneNumber),
            nameof(TenantUser.Tenant_Id),
            "Discriminator"
        ];
         
        builder
            .HasIndex(emailUniqueIndexProperties)
            .IsUnique();

        builder
            .HasIndex(phoneNumberUniqueIndexProperties)
            .IsUnique();
    }
}
