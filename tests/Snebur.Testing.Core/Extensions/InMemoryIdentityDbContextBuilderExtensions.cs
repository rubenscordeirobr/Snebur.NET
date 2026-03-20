using Snebur.Persistence.Identity.Seed;
using Snebur.Testing.Core.EFCore;
using Snebur.Testing.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Snebur.Testing.Core.Extensions;

public static class InMemoryIdentityDbContextBuilderExtensions
{
    public static IServiceCollection AddInMemoryIdentityDbContext(
           this IServiceCollection services)
    {
        services
            .AddSingleton<IModelCustomizer, InMemoryIdentityDbContextModelCustomizer>()
            .AddDbContext<IdentityDbContext>(
                optionsBuilder =>
                {
                    optionsBuilder.UseInMemoryDatabase(
                        databaseName: "IdentityDbMemory_" + services.GetHashCode(),
                        optionsBuilder =>
                        {
                            optionsBuilder.ConfigureEnumMappings<IdentityDbContext>(isInMemory: true);
                        });
                    optionsBuilder.ReplaceService<IModelCustomizer, InMemoryIdentityDbContextModelCustomizer>();
                },
                contextLifetime: ServiceLifetime.Transient,
                optionsLifetime: ServiceLifetime.Singleton);
         

        using (var serviceProvider = services.BuildServiceProvider())
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
                var secureConfiguration = scope.ServiceProvider.GetRequiredService<ISecureConfiguration>();
            
                context.Database.EnsureCreated();
                context.SeedAsync(secureConfiguration).Wait();
            }
        }

        return services;
    }

    public static DbContextOptionsBuilder UseInMemoryDatabase(
        this DbContextOptionsBuilder optionsBuilder,
        string databaseName,
        Action<RelationalInMemoryDbContextOptionsBuilder> inMemoryOptionsBuilderAction)
    {
        lock (optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(databaseName);
            var inMemoryOptions = new RelationalInMemoryDbContextOptionsBuilder(optionsBuilder);
            inMemoryOptionsBuilderAction?.Invoke(inMemoryOptions);
        }
        return optionsBuilder;
    }
}
