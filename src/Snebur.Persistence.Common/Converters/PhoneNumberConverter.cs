using Snebur.SharedKernel.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Snebur.Persistence.Common.Converters;

internal class PhoneNumberConverter : ValueConverter<PhoneNumber, string>
{
    private PhoneNumberConverter() : base(
        phoneNumber => phoneNumber.FullNumber,
        value => new PhoneNumber(value) )
    {
    }

    internal static PhoneNumberConverter Instance { get; } = new();
}
