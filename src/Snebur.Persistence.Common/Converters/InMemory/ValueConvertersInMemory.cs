namespace Snebur.Persistence.Common.Converters.InMemory;

internal static class ValueConvertersInMemory
{
    internal static GeoLocationConverter GeoLocationConverter 
        => GeoLocationConverter.Instance;

    internal static PasswordConverter PasswordConverter 
        => PasswordConverter.Instance;

    internal static TimeZoneOffsetConverter TimeZoneOffsetConverter 
        => TimeZoneOffsetConverter.Instance;
}
