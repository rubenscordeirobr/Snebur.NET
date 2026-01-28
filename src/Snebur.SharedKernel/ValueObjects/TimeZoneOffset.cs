using System.Globalization;
using System.Text.Json.Serialization;
using Snebur.Core.Utils;

namespace Snebur.SharedKernel.ValueObjects;

public sealed record TimeZoneOffset : ValueObjectBase
{
    public string Offset { get; private set; }
    public string Location { get; private set; }

    [JsonIgnore]
    public TimeSpan OffsetTimeSpan
        => TimeSpan.Parse(Offset, CultureInfo.InvariantCulture);

    [JsonConstructor]
    private TimeZoneOffset(string offset, string location)
    {
        Offset = offset;
        Location = location;
    }

    public sealed override string ToString()
        => $"Offset: {Offset} Location:{Location}";

    public string ToJson()
        => JsonUtils.Serialize(this);

    public static Result<TimeZoneOffset> Create(string offset, string location)
    {
        if (string.IsNullOrWhiteSpace(offset))
        {
            return Result.ValidationFailure<TimeZoneOffset>(
                "TimeZone.OffsetEmpty",
                "Offset cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(location))
        {
            return Result.ValidationFailure<TimeZoneOffset>(
                "TimeZine.LocationEmpty",
                "Location cannot be empty.");
        }

        if (!TimeSpan.TryParse(offset, CultureInfo.InvariantCulture, out var _))
        {
            return Result.ValidationFailure<TimeZoneOffset>(
                "TimeZone.InvalidOffset",
                $"Invalid offset. {offset}");
        }
        return Result.Success(new TimeZoneOffset(offset, location));
    }

    public static TimeZoneOffset Default
        => new("00:00", "UTC");

    public static TimeZoneOffset Parse(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return Default;

        return JsonUtils.Deserialize<TimeZoneOffset>(json)
            ?? Default;
    }
}
