using System.Reflection;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Snebur.Persistence.Common.Validations;

public class NotEmptyGuidValidation : ValueConverter<Guid, Guid>
{
    public NotEmptyGuidValidation(PropertyInfo property) : base(
        guid => Convert(guid, property),
        guid => Convert(guid, property))
    {
    }

    internal static Guid Convert(Guid value, PropertyInfo property)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException($"Guid value {property.GetPropertyPath()} cannot be empty.");
        }
        return value;
    }
}
