using Snebur.Core.Extensions;
using Snebur.Testing.Core.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Snebur.Testing.Core.Extensions;

public static class InMemoryEntityBuilderConfiguration
{
    public static ModelBuilder ConfigureInMemoryEntities(
      this ModelBuilder modelBuilder)
    {
        Guard.NotNull(modelBuilder);

        var entitiesType = modelBuilder.Model.GetEntityTypes();
        foreach (var mutableEntityType in entitiesType)
        {
            var entityType = mutableEntityType.ClrType;
            if (!entityType.IsSubclassOf<EntityBase>())
            {
                continue;
            }
            var entityBuilder = modelBuilder.Entity(entityType);
            entityBuilder.ConfigureInMemoryEntity();
        }
        return modelBuilder;
    }

    public static EntityTypeBuilder ConfigureInMemoryEntity(
        this EntityTypeBuilder entityBuilder)
    {
        Guard.NotNull(entityBuilder);

        var entityType = entityBuilder.Metadata.ClrType;
        Guard.NotNull(entityType);

        if (!entityType.IsSubclassOfOrEquals<EntityBase>())
        {
            throw new InvalidOperationException($"The entity {entityType.Name} must be a subclass of EntityBase");
        }

        var idProperty = entityBuilder.Property(nameof(EntityBase.Id));

        Guard.NotNull(idProperty);

        idProperty
            .HasValueGenerator<GuidIntegerValueGenerator>()
            .ValueGeneratedOnAdd();

        entityBuilder.ConfigureDateTimeDefaultValues();

      
        return entityBuilder;
    }

    private static EntityTypeBuilder ConfigureDateTimeDefaultValues(
        this EntityTypeBuilder entityBuilder)
    {
        var proprities = entityBuilder.Metadata.GetProperties();
        foreach (var property in proprities)
        {
            var sqlValueGenerated = property.GetDefaultValueSql();
            if (sqlValueGenerated == "now()")
            {
                var propertyBuilder = entityBuilder.Property(property.Name);
                propertyBuilder
                    .HasValueGenerator<DateTimeNowValueGenerator>()
                    .ValueGeneratedOnAddOrUpdate();
            }
        }
        return entityBuilder;
    }
}
