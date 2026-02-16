using Snebur.SharedKernel.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Snebur.Persistence.Common.Converters.InMemory;

internal class GeoLocationConverter : ValueConverter<GeoLocation, string>
{
    private GeoLocationConverter() : base(
        geoLocation => geoLocation.ToJson(),
        json => GeoLocation.Parse(json))
    {
    }

    internal static GeoLocationConverter Instance { get; } = new();
}
