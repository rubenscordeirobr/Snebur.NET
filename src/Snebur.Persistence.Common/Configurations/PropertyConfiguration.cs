using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Snebur.Persistence.Common.Converters;
using Snebur.Persistence.Common.Validations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Snebur.Persistence.Common.Configurations;

internal static class PropertyConfiguration
{
    internal static IMutableEntityType ConfigureProperties(this IMutableEntityType mutableEntityType)
    {
        foreach (var mutableProperty in mutableEntityType.GetProperties())
        {
            mutableProperty
                .ConfigureCollation()
                .ConfigureValueConverter();
        }
        return mutableEntityType;
    }

    internal static IMutableProperty ConfigureCollation(this IMutableProperty mutableProperty)
    {
        if (mutableProperty.ClrType == typeof(string) &&
            mutableProperty.GetCollation() == null)
        {
            mutableProperty.SetCollation("case_accent_insensitive");
        }
        return mutableProperty;
    }

    internal static IMutableProperty ConfigureValueConverter
        (this IMutableProperty mutableProperty)
    {
        if (mutableProperty.ClrType == typeof(DateTime))
        {
            mutableProperty.SetValueConverter(ValueConverters.UtcDateTimeConverter);
        }

        if (mutableProperty.ClrType == typeof(DateTime?))
        {
            mutableProperty.SetValueConverter(ValueConverters.NullableUtcDateTimeConverter);
        }
        if (mutableProperty.ClrType == typeof(Guid?))
        {
            mutableProperty.SetValueConverter(ValueConverters.EmptyNullableGuidConverter);
        }

        if (mutableProperty.ClrType == typeof(Guid) &&
            RequiresNotEmptyGuidValidation(mutableProperty.PropertyInfo))
        {
            mutableProperty.SetValueConverter(new NotEmptyGuidValidation(mutableProperty.PropertyInfo));

        }
        return mutableProperty;
    }

    private static bool RequiresNotEmptyGuidValidation(
        [NotNullWhen(true)] PropertyInfo? propertyInfo)
    {
        if (propertyInfo is null)
            return false;

        var isGuidType = propertyInfo.PropertyType == typeof(Guid);
        if (!isGuidType)
            return false;

        if (propertyInfo.DeclaringType == typeof(EntityBase) &&
            propertyInfo.Name == nameof(EntityBase.Id))
            return false;

        return true;
    }
}
