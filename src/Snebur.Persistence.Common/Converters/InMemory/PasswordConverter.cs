using Snebur.SharedKernel.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Snebur.Persistence.Common.Converters.InMemory;

internal class PasswordConverter: ValueConverter<Password, string>
{
    private PasswordConverter() : base(
        password => password.ToJson(),
        json => Password.Parse(json))
    {
    }

    internal static PasswordConverter Instance { get; } = new();
}
