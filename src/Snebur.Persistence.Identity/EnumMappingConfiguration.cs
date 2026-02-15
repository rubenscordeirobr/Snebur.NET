using Snebur.Persistence.Common.Configurations;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Snebur.Persistence.Identity;

public static partial class EnumMappingConfiguration
{
    public static void ConfigureEnumMappings<TContext>(
        this IRelationalDbContextOptionsBuilderInfrastructure  optionsBuilder,
        bool isInMemory = false)
        where TContext : DbContext 
    {
        optionsBuilder
            .AddMapEnum<TContext, UserStatus>(isInMemory)
            .AddMapEnum<TContext, UserRole>(isInMemory)
            .AddMapEnum<TContext, UserType>(isInMemory)
            .AddMapEnum<TContext, AuthenticationType>(isInMemory)
            .AddMapEnum<TContext, BusinessType>(isInMemory)
            .AddMapEnum<TContext, Country>(isInMemory)
            .AddMapEnum<TContext, Currency>(isInMemory)
            .AddMapEnum<TContext, Culture>(isInMemory)
            .AddMapEnum<TContext, Language>(isInMemory)
            .AddMapEnum<TContext, PasswordStrength>(isInMemory)
            .AddMapEnum<TContext, TenantState>(isInMemory)
            .AddMapEnum<TContext, TenantStatus>(isInMemory)
            .AddMapEnum<TContext, TenantType>(isInMemory)
            .AddMapEnum<TContext, UserState>(isInMemory)
            .AddMapEnum<TContext, VerificationState>(isInMemory);
    }
}
