using Snebur.Testing.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Snebur.Testing.Core.EFCore;

public class InMemoryIdentityDbContextModelCustomizer : IModelCustomizer
{
    public void Customize(ModelBuilder modelBuilder, DbContext context)
    {
        if (context is IdentityDbContext _)
        {
            modelBuilder
                .ConfigureModelDefaultConfiguration<IdentityDbContext>(isInMemory: true)
                .ConfigureInMemoryEntities();
        }
    }
}
