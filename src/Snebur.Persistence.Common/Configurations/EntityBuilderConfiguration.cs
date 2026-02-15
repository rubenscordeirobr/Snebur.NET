using Snebur.Persistence.Common.Helpers;

namespace Snebur.Persistence.Common.Configurations;

public static class EntityBuilderConfiguration
{
    public static EntityTypeBuilder ConfigureEntityDefaultSettings(
        this EntityTypeBuilder entityBuilder,
        bool isInMemory)
    {
        Guard.NotNull(entityBuilder);

        return entityBuilder.ConfigureEntityBase(isInMemory)
                .ConfigureSoftDeletableEntity()
                .ConfigureSortable()
                .ConfigureIndexes()
                .ConfigureRestrictDeleteBehavior()
                .ConfigureNotEmptyGuidConstraint();
    }

    private static EntityTypeBuilder ConfigureEntityBase(
        this EntityTypeBuilder entityBuilder,
        bool isInMemory)
    {
        var entityType = entityBuilder.Metadata.ClrType;

        Guard.NotNull(entityType);

        if (!entityType.IsSubclassOfOrEquals<EntityBase>())
        {
            throw new InvalidEntityTypeException($"The entity {entityType.Name} must be a subclass of EntityBase");
        }

        var baseType = entityType.BaseType;
        if (baseType == typeof(EntityBase))
        {
            entityBuilder.HasKey(nameof(EntityBase.Id));
            entityBuilder.ConfigureNotEmptyGuidConstraint(baseType);
        }

        if (!isInMemory)
        {
            entityBuilder.Property(nameof(EntityBase.Id))
              .ValueGeneratedOnAdd()
              .HasDefaultValueSql("uuid_generate_v4()");
        } 

        entityBuilder.Property(nameof(EntityBase.CreatedAt))
               .HasDefaultValueSql("now()")
               .ValueGeneratedOnAdd();

        entityBuilder.Property(nameof(EntityBase.LastUpdatedAt))
            .HasDefaultValueSql("now()")
            .ValueGeneratedOnAdd();

        entityBuilder.Property(nameof(EntityBase.CreatedSession_Id))
            .IsRequired();

        entityBuilder.Property(nameof(EntityBase.CreatedSession_Id))
            .IsRequired();

        return entityBuilder;
    }

    private static EntityTypeBuilder ConfigureSoftDeletableEntity(
        this EntityTypeBuilder entityBuilder)
    {
        var isSoftDeletable = typeof(ISoftDeletableEntity).IsAssignableFrom(entityBuilder.Metadata.ClrType);

        if (isSoftDeletable)
        {
            entityBuilder.Property(nameof(ISoftDeletableEntity.IsDeleted))
                .IsRequired()
                .HasDefaultValue(false);

            entityBuilder.Property(nameof(ISoftDeletableEntity.DeletedAt))
                .IsRequired(false);

            entityBuilder.Property(nameof(ISoftDeletableEntity.DeletedSession_Id))
                .IsRequired(false);

            return entityBuilder;
        }
        return entityBuilder;
    }

    private static EntityTypeBuilder ConfigureSortable(
        this EntityTypeBuilder entityBuilder)
    {
        var entityType = entityBuilder.Metadata;
        var isSortable = typeof(ISortable).IsAssignableFrom(entityType.ClrType);
        if (isSortable)
        {
            var isDescending = typeof(IDescendingSortable).IsAssignableFrom(entityType.ClrType);

            var sortProperty = entityType.FindProperty(nameof(ISortable.SortOrder))
                ?? throw new SortablePropertyMissingException($"The entity {entityType.ClrType.Name} implements ISortable but does not have a property named 'SortOrder'");

            var propertyBuilder = entityBuilder.Property(nameof(ISortable.SortOrder));

            var tableName = entityType.GetTableName();
            var columnName = sortProperty.GetColumnName();

            var defaultValueFunctionName = isDescending
                ? $"get_next_sort_order_desc('{tableName}', '{columnName}')"
                : $"get_next_sort_order_asc('{tableName}', '{columnName}')";

            propertyBuilder.HasDefaultValueSql(defaultValueFunctionName);
            entityBuilder.HasIndex(nameof(ISortable.SortOrder));
        }
        return entityBuilder;
    }

    private static EntityTypeBuilder ConfigureIndexes(
            this EntityTypeBuilder entityBuilder)
    {
        var entityType = entityBuilder.Metadata;
        var isDeleted = typeof(ISoftDeletableEntity).IsAssignableFrom(entityType.ClrType);

        foreach (var index in entityType.GetIndexes())
        {
            foreach (var indexProperty in index.Properties)
            {
                if (indexProperty.GetCollation() == null &&
                    indexProperty.ClrType == typeof(string))
                {
                    indexProperty.SetCollation("case_accent_insensitive");
                }
            }

            if (isDeleted && index.IsUnique)
            {
                var indexBuilder = entityBuilder.HasIndex([.. index.Properties.Select(x => x.Name)]);
                var deletedColumnName = entityBuilder
                    .Property(nameof(ISoftDeletableEntity.IsDeleted))
                    .Metadata
                    .GetColumnName();

                var softDeletedFilter = FilterExpressionHelper.AppendSoftDeleteFilter(
                    index.GetFilter(), 
                    deletedColumnName);

                indexBuilder.HasFilter(softDeletedFilter);
            }
        }
        return entityBuilder;
    }

    private static EntityTypeBuilder ConfigureRestrictDeleteBehavior(
        this EntityTypeBuilder entityBuilder)
    {
    
        foreach (var relationship in entityBuilder.Metadata.GetForeignKeys())
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }
        return entityBuilder;
    }

    private static EntityTypeBuilder ConfigureNotEmptyGuidConstraint(
       this EntityTypeBuilder entityBuilder)
    {
        var entityType = entityBuilder.Metadata.ClrType;
        Guard.NotNull(entityType);
        entityBuilder.ConfigureNotEmptyGuidConstraint(entityType);
        return entityBuilder;
    }

    private static EntityTypeBuilder ConfigureNotEmptyGuidConstraint(
       this EntityTypeBuilder entityBuilder,
       Type entityType)
    {
        var guidProperties = entityType.GetDeclaredPropertiesOfType<Guid>();
        foreach (var property in guidProperties)
        {
            var propertyBuilder = entityBuilder.Property(property.Name);
            var columnName = propertyBuilder.Metadata.GetColumnName();

            entityBuilder.ToTable((tableBuilder) =>
            {
                tableBuilder.HasCheckConstraint($"ck_{tableBuilder.Metadata.GetTableName()}_{columnName}_not_empty",
                    $"{columnName} <> '00000000-0000-0000-0000-000000000000'::uuid");

            });
        }
        return entityBuilder;
    }
}
