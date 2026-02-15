namespace Snebur.Persistence.Common.Converters;

internal static class ValueConverters
{
    internal static PhoneNumberConverter PhoneNumberConverter
        => PhoneNumberConverter.Instance;

    internal static FiscalCodeConverter FiscalCodeConverter
        => FiscalCodeConverter.Instance;

    internal static NullableUtcDateTimeConverter NullableUtcDateTimeConverter
        => NullableUtcDateTimeConverter.Instance;

    internal static UtcDateTimeConverter UtcDateTimeConverter 
        => UtcDateTimeConverter.Instance;

    internal static NullableGuidValueConverter EmptyNullableGuidConverter
        => NullableGuidValueConverter.Instance;
}
