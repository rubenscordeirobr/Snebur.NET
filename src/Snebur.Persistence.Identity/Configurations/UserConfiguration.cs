namespace Snebur.Persistence.Identity.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        Guard.NotNull(builder);

        builder.Property(x => x.Role)
          .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(ValidationConstants.NameMaxLength);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(ValidationConstants.EmailMaxLength);
    }
}
