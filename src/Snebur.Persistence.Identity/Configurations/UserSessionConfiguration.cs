namespace Snebur.Persistence.Identity.Configurations;

public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        Guard.NotNull(builder);
          
        builder.Property(x => x.ApplicationName)
            .IsRequired()
            .HasMaxLength(ValidationConstants.NameMaxLength);

        builder.Property(x => x.IpAddress)
            .IsRequired()
            .HasMaxLength(ValidationConstants.IpAddressMaxLength);
         
        builder.Property(x => x.UserAgent)
            .IsRequired()
            .HasMaxLength(ValidationConstants.UserAgentMaxLength);

        builder.Property(x => x.StartedAt)
            .HasDefaultValueSql("now()")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.LastActivity)
            .HasDefaultValueSql("now()")
            .ValueGeneratedOnAdd();

        builder.HasOne(x => x.User)
           .WithMany(x => x.Sessions)
           .HasForeignKey(x => x.User_Id)
           .IsRequired(false);

        builder.HasOne(x => x.Tenant)
           .WithMany(x => x.Sessions)
           .HasForeignKey(x => x.Tenant_Id)
           .IsRequired(false);
         
    }
}
