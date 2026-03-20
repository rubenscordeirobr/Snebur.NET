using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Snebur.Testing.Core.EFCore;

public class RelationalInMemoryDbContextOptionsBuilder : InMemoryDbContextOptionsBuilder,
    IRelationalDbContextOptionsBuilderInfrastructure
{
    public RelationalInMemoryDbContextOptionsBuilder(DbContextOptionsBuilder optionsBuilder)
        : base(optionsBuilder)
    {
    }

    DbContextOptionsBuilder IRelationalDbContextOptionsBuilderInfrastructure.OptionsBuilder
        => OptionsBuilder;
}
