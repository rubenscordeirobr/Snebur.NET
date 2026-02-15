using System.Reflection;
using Snebur.Persistence.Common.Converters;
using Snebur.SharedKernel.ValueObjects;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Snebur.Persistence.Common.Configurations;

public static class ValueObjectsConfiguration
{
    public static IMutableEntityType ConfigureValueObjects(
        this IMutableEntityType mutableEntityType,
        EntityTypeBuilder entityBuilder,
        bool isInMemory)
    {
        Guard.NotNull(entityBuilder);
        Guard.NotNull(mutableEntityType);

        var entityType = mutableEntityType.ClrType;
        if (!entityType.IsSubclassOf<EntityBase>())
        {
            throw new NotSupportedException($"Entity {entityType.Name} is not a subclass of EntityBase");
        }

        var valueObjectProperties = entityType.GetDeclaredPropertiesOfType<ValueObjectBase>();
        foreach (var property in valueObjectProperties)
        {
            entityBuilder.ConfigureValueObjects(property, isInMemory);
        }
        return mutableEntityType;
    }
     
    private static EntityTypeBuilder ConfigureValueObjects(
        this EntityTypeBuilder entityBuilder,
        PropertyInfo property,
        bool isDatabaseInMemory)
    {
        if (isDatabaseInMemory)
        {
            return entityBuilder.ConfigureValueObjectsInMemory(property);
        }

        switch (property.PropertyType)
        {
            case Type t when t == typeof(TimeZoneOffset):
                {
                    entityBuilder.ComplexProperty<TimeZoneOffset>(
                        property.Name, ConfigureTimeZoneOffset);
                }
                break;

            case Type t when t == typeof(Password):

                entityBuilder.ComplexProperty<Password>(
                    property.Name, ConfigurePassword);

                break;

            case Type t when t == typeof(GeoLocation):

                entityBuilder.ComplexProperty<GeoLocation>(
                    property.Name, ConfigureGeoLocation);

                break;
            case Type t when t == typeof(FiscalCode):

                ConfigureFiscalCode(entityBuilder, property);
                break;
            case Type t when t == typeof(PhoneNumber):

                ConfigurePhoneNumber(entityBuilder, property);

                break;

            default:
                throw new NotSupportedException($"Value object {property.Name} not supported");
        }
        return entityBuilder;
    }
     
    internal static void ConfigureTimeZoneOffset(
              ComplexPropertyBuilder<TimeZoneOffset> builder)
    {
        builder.Property(tz => tz.Offset)
               .HasColumnName("Offset")
               .HasMaxLength(6) // ±hh:mm
               .IsRequired();

        // Map the Location property
        builder.Property(tz => tz.Location)
            .HasColumnName("TimeZone_Location")
            .HasMaxLength(100)
            .IsRequired();

        builder.Ignore(t => t.OffsetTimeSpan);
    }

    internal static void ConfigurePassword(
        ComplexPropertyBuilder<Password> builder)
    {
        builder.Property(p => p.HashValue)
            .HasMaxLength(ValidationConstants.PasswordHashLength)
            .IsRequired();

        builder.Property(p => p.Strength)
            .IsRequired();
    }

    internal static void ConfigureGeoLocation(
        ComplexPropertyBuilder<GeoLocation> builder)
    {
        builder.Property(gl => gl.Latitude)
            .IsRequired(false);

        builder.Property(gl => gl.Longitude)
            .IsRequired(false);
    }

    private static void ConfigurePhoneNumber(
        EntityTypeBuilder entityBuilder,
        PropertyInfo property)
    {
        entityBuilder.Property<PhoneNumber>(property.Name)
            .IsRequired()
           .HasConversion(ValueConverters.PhoneNumberConverter)
           .HasMaxLength(ValidationConstants.PhoneNumberMaxLength);
    }

    private static void ConfigureFiscalCode(
       EntityTypeBuilder entityBuilder,
       PropertyInfo property)
    {
        Guard.NotNull(entityBuilder);
        Guard.NotNull(property);

        entityBuilder.Property<FiscalCode>(property.Name)
           .IsRequired()
           .HasConversion(ValueConverters.FiscalCodeConverter)
           .HasMaxLength(ValidationConstants.FiscalCodeMaxLength);
    }
}
