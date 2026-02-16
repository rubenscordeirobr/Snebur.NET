using Snebur.Persistence.Common.Converters;
using Snebur.Persistence.Common.Converters.InMemory;
using Snebur.SharedKernel.ValueObjects;
using System.Reflection;

namespace Snebur.Persistence.Common.Configurations;

public static class ValueObjectsConfigurationInMemory
{
    public static EntityTypeBuilder ConfigureValueObjectsInMemory(
        this EntityTypeBuilder entityBuilder,
        PropertyInfo property )
    {
        Guard.NotNull(entityBuilder);
        Guard.NotNull(property);

        switch (property.PropertyType)
        {
            case Type t when t == typeof(TimeZoneOffset):

                ConfigureTimeZoneOffset(entityBuilder, property);
                break;

            case Type t when t == typeof(Password):

                ConfigurePassword(entityBuilder, property);

                break;
            case Type t when t == typeof(GeoLocation):
 
                ConfigureGeoLocation(entityBuilder, property);

                break;
            case Type t when t == typeof(PhoneNumber):

                ConfigurePhoneNumber(entityBuilder, property);

                break;
            case Type t when t == typeof(FiscalCode):

                ConfigureFiscalCode(entityBuilder, property);

                break;

            default:
                throw new NotSupportedException($"Value object {property.Name} not supported");
        }
        return entityBuilder;
    }

    private static void ConfigureTimeZoneOffset(EntityTypeBuilder entityBuilder, PropertyInfo property)
    {
        entityBuilder.Property(property.Name)
            .IsRequired()
            .HasConversion(ValueConvertersInMemory.TimeZoneOffsetConverter)
            .HasMaxLength(255);
    }

    private static void ConfigurePassword(EntityTypeBuilder entityBuilder, PropertyInfo property)
    {
        entityBuilder.Property(property.Name)
            .IsRequired()
            .HasConversion(ValueConvertersInMemory.PasswordConverter)
            .HasMaxLength(ValidationConstants.PasswordMaxLength + 30);
    }

    private static void ConfigureGeoLocation(EntityTypeBuilder entityBuilder, PropertyInfo property)
    {
        entityBuilder.Property(property.Name)
            .IsRequired()
            .HasConversion(ValueConvertersInMemory.GeoLocationConverter)
            .HasMaxLength(255);
    }

    private static void ConfigurePhoneNumber(EntityTypeBuilder entityBuilder, PropertyInfo property)
    {
        entityBuilder.Property(property.Name)
           .IsRequired()
           .HasConversion(ValueConverters.PhoneNumberConverter)
           .HasMaxLength(ValidationConstants.PhoneNumberMaxLength);
    }

    private static void ConfigureFiscalCode(EntityTypeBuilder entityBuilder, PropertyInfo property)
    {
        entityBuilder.Property(property.Name)
           .IsRequired()
           .HasConversion(ValueConverters.FiscalCodeConverter)
           .HasMaxLength(ValidationConstants.FiscalCodeMaxLength);
    }
}
