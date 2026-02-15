using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Snebur.Persistence.Common.Converters;

internal class UtcDateTimeConverter : ValueConverter<DateTime, DateTime>
{
    private UtcDateTimeConverter() : base(
        v => ConverterUtcDateTime(v),
        v => ConverterUtcDateTime(v))
    {
    }
     
    private static DateTime ConverterUtcDateTime(DateTime dateTime)
    {
        if(dateTime == default)
        {
            return DateTime.SpecifyKind(default, DateTimeKind.Utc);
        }

        return dateTime.Kind == DateTimeKind.Utc 
            ? dateTime 
            : DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
    }

    internal static UtcDateTimeConverter Instance { get; } = new UtcDateTimeConverter();
}

