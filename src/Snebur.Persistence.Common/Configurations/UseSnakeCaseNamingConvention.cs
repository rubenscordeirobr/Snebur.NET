using Snebur.Core.Utils;

namespace Snebur.Persistence.Common.Configurations;

internal static class UseSnakeCaseNamingConventionExtensions
{
    internal static void UseSnakeCaseNamingConvention(this ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = CaseConventionUtils.ToSnakeCase(entity.GetTableName() ?? string.Empty);
            entity.SetTableName(tableName);

            foreach (var property in entity.GetProperties())
            {
                var columnName = property.GetColumnName();
                if (!string.IsNullOrWhiteSpace(columnName))
                {
                    property.SetColumnName(CaseConventionUtils.ToSnakeCase(columnName));
                }
            }

            foreach (var index in entity.GetIndexes())
            {
                var indexName = index.GetDatabaseName();
                if (!string.IsNullOrWhiteSpace(indexName))
                {
                    index.SetDatabaseName(CaseConventionUtils.ToSnakeCase(indexName));
                }
            }

            foreach (var key in entity.GetKeys())
            {
                var keyName = key.GetName();
                if (!string.IsNullOrWhiteSpace(keyName))
                {
                    key.SetName(CaseConventionUtils.ToSnakeCase(keyName));
                }
            }

            foreach (var foreignKey in entity.GetForeignKeys())
            {
                var foreignKeyName = foreignKey.GetConstraintName();
                if (!string.IsNullOrWhiteSpace(foreignKeyName))
                {
                    foreignKey.SetConstraintName(CaseConventionUtils.ToSnakeCase(foreignKeyName));
                }
            }
        }
    }
}
