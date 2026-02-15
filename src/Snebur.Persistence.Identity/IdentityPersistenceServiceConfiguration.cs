using Snebur.Application.Abstractions.Security;
using Snebur.Persistence.Common.Interceptors;
using Snebur.Persistence.Identity.Repositories;
using Snebur.Persistence.Identity.Seed;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Snebur.Persistence.Identity;

public static class IdentityPersistenceServiceConfiguration
{
    public static IServiceCollection AddIdentityPersistenceServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        return services.AddNpgsqlIdentityDbContext(configuration, environment)
            .AddIdentityRepositoryServices();
    }

    private static IServiceCollection AddNpgsqlIdentityDbContext(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var connectionString = configuration.GetConnectionString("IdentityPostgresql");

        return services.AddDbContext<IdentityDbContext>(optionsBuilder =>
        {
            optionsBuilder.UseNpgsql(connectionString, npgOptionsBuilder =>
            {
                npgOptionsBuilder.ConfigureEnumMappings<IdentityDbContext>();
            });

            optionsBuilder.AddInterceptors(new DefaultValuesInterceptor())
                .AddInterceptors(new DefaultSaveChangesInterceptor());

#if DEBUG
            if (environment.IsDevelopment())
            {
                optionsBuilder.EnableSensitiveDataLogging();
            }
#endif

        },
        contextLifetime: ServiceLifetime.Scoped,
        optionsLifetime: ServiceLifetime.Singleton
        );
    }

    internal static IServiceCollection AddIdentityRepositoryServices(this IServiceCollection services)
    {
        return services.AddScoped<IAdminUserRepository, AdminUserRepository>()
             .AddScoped<ITenantUserRepository, TenantUserRepository>()
             .AddScoped<ISystemUserRepository, SystemUserRepository>()
             .AddScoped<ITenantRepository, TenantRepository>()
             .AddScoped<IUserSessionRepository, UserSessionRepository>()
             .AddTransient<IIdentityUnitOfWork, IdentityUnitOfWork>();
    }

    public static async Task ApplyMigrationsAsync(this IApplicationBuilder app)
    {
        Guard.NotNull(app);

        using var scope = app.ApplicationServices.CreateScope();

        var services = scope.ServiceProvider;
        var dbContext = services.GetRequiredService<IdentityDbContext>();
        var secureConfiguration = services.GetRequiredService<ISecureConfiguration>();

        await dbContext.Database.MigrateAsync();
        await dbContext.SeedAsync(secureConfiguration);
    }
}
