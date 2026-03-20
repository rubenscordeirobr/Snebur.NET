using Snebur.Core.Helpers;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Snebur.Testing.Core.EFCore;

public class GuidIntegerValueGenerator : ValueGenerator<Guid>
{
    public override Guid Next(EntityEntry entry)
    {
        Guard.NotNull(entry);

        if (entry.Entity is EntityBase entity &&
            entity.Id != Guid.Empty)
        {
            return entity.Id;
        }
        return GuidHelper.NewGuidZeroPrefixed();
    }

    public override bool GeneratesTemporaryValues
        => false;
}
