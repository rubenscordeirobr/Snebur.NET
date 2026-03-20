using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Snebur.Testing.Core.EFCore;

public class DateTimeNowValueGenerator : ValueGenerator<DateTime>
{
    public override DateTime Next(EntityEntry entry)
        => DateTime.UtcNow;

    public override bool GeneratesTemporaryValues
        => false;
}
