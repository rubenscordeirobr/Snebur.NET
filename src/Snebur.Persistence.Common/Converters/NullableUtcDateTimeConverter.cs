using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Snebur.Persistence.Common.Converters;

internal class NullableUtcDateTimeConverter : ValueConverter<DateTime?, DateTime?>
{
    private NullableUtcDateTimeConverter() : base(
        v => ConverterUtcDateTime(v),
        v => ConverterUtcDateTime(v))
    {
    }

    private static DateTime? ConverterUtcDateTime(DateTime? dateTime)
    {
        if (dateTime == null || dateTime.Value == default )
        {
            return null;
        }

        return dateTime.Value.Kind == DateTimeKind.Utc
            ? dateTime
            : DateTime.SpecifyKind(dateTime.Value, DateTimeKind.Utc);
    }

    public static NullableUtcDateTimeConverter Instance { get; } = new NullableUtcDateTimeConverter();
}

