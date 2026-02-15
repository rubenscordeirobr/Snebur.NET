using Snebur.Application.Exceptions;
using Snebur.Persistence.Common.Configurations;

namespace Snebur.Persistence.Identity;

internal class IdentityDbContext : DbContext, IDbSeedAsync
{
    internal DbSet<User> Users { get; set; }
    internal DbSet<UserSession> Sessions { get; set; }
    internal DbSet<Tenant> Tenants { get; set; }
    internal DbSet<TenantAddress> Addresses { get; set; }

    public IdentityDbContext(
        DbContextOptions<IdentityDbContext> options ) 
        : base(options)
    {
    }

    public override int SaveChanges()
    {
        throw new SyncSaveChangesNotAllowedException("Use SaveChangesAsync instead");
    }

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }

  
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigureModelDefaultConfiguration<IdentityDbContext>(isInMemory:false);
        base.OnModelCreating(modelBuilder);
    }

    #region IDbSeedAsync 

    async Task<int> IDbSeedAsync.SeedSaveChangesAsync()
    {
        if (await this.Users.AnyAsync())
        {
            throw new InvalidOperationException("Database already seeded");
        }
        return await base.SaveChangesAsync();
    } 
    #endregion
}

internal interface IDbSeedAsync
{
    Task<int> SeedSaveChangesAsync();
}
