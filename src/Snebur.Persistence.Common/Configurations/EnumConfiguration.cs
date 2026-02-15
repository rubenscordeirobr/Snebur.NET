using System.Collections.Concurrent;
using Snebur.Core.Collections;
using Snebur.Persistence.Common.Exceptions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace Snebur.Persistence.Common.Configurations;

public static class EnumConfiguration
{
    private static readonly ConcurrentDictionary<Type, ConcurrentHashSet<Type>> _enumTypeMappings = new();

    public static IRelationalDbContextOptionsBuilderInfrastructure AddMapEnum<TContext, TEnum>(
         this IRelationalDbContextOptionsBuilderInfrastructure optionsBuilder,
         bool isInMemory)
         where TContext : DbContext
         where TEnum : struct, Enum
    {
        if (!isInMemory &&
            optionsBuilder is NpgsqlDbContextOptionsBuilder npgsqlOptionsBuilder)
        {
            npgsqlOptionsBuilder.MapEnum<TEnum>();
        }

#if DEBUG
        EnumMappingsTracker<TContext>.AddEnumType<TEnum>();
#endif
        return optionsBuilder;
    }

    public static void CheckEnums<TContext>(
       this IMutableProperty mutableProperty)
       where TContext : DbContext

    {
        Guard.NotNull(mutableProperty);
#if DEBUG
        if (mutableProperty.ClrType.IsEnum &&
            !EnumMappingsTracker<TContext>.Contains(mutableProperty.ClrType))
        {
            var declaredTypeName = mutableProperty.PropertyInfo?.DeclaringType?.GetQualifiedName() ?? "Unknown";
            var errorMessages = $"The enum type '{mutableProperty.ClrType?.Name}' not mapped." +
                                $" The property '{declaredTypeName}.{mutableProperty.Name}' must map the enum in NpgsqlDbContextOptionsBuilder using AddMapEnum method";

            throw new EnumTypeNotMappedException(errorMessages);
        }

#endif
    }

#if DEBUG

    private static class EnumMappingsTracker<TContext>
        where TContext : DbContext
    {
        private static readonly Type _dbContextType = typeof(TContext);
        private static ConcurrentHashSet<Type> EnumTypesMapped
            => _enumTypeMappings.GetOrAdd(_dbContextType, []);

        public static void AddEnumType<TEnum>()
            where TEnum : Enum
        {
            EnumTypesMapped.Add(typeof(TEnum));
        }

        internal static bool Contains(Type enumType)
        {
            return EnumTypesMapped.Contains(enumType);
        }
    }

#endif
}

