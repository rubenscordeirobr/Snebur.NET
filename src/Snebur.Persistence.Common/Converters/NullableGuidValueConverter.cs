using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Snebur.Persistence.Common.Converters;

public class NullableGuidValueConverter : ValueConverter<Guid?, Guid?>
{
    private NullableGuidValueConverter() : base(
        guid => Convert(guid),
        guid => Convert(guid))
    {
    }

    internal static Guid? Convert(Guid? value)
    {
        return value == null || value.Value == Guid.Empty
            ? null
            : value;
    }

    public static NullableGuidValueConverter Instance { get; } = new();
}
