using Snebur.SharedKernel.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Snebur.Persistence.Common.Converters.InMemory;

internal class TimeZoneOffsetConverter : ValueConverter<TimeZoneOffset, string>
{
    private TimeZoneOffsetConverter() : base(
        timeZoneOffset => timeZoneOffset.ToJson(),
        json => TimeZoneOffset.Parse(json))
    {
    }
    internal static TimeZoneOffsetConverter Instance { get; } = new();
}
