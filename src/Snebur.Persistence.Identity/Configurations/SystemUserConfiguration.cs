namespace Snebur.Persistence.Identity.Configurations;

public class SystemUserConfiguration : IEntityTypeConfiguration<SystemUser>
{
    public void Configure(EntityTypeBuilder<SystemUser> builder)
    {
        Guard.NotNull(builder);

        builder.HasBaseType<User>();
    }
}
