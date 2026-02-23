using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Snebur.ArchitectureTests;

public class EntityValidationTests : IClassFixture<ApplicationAssemblyContext>
{
    private readonly ITestOutputHelper _output;
    private readonly IReadOnlyDictionary<Type, Type> _entityTypeToConfigurationTypeMap;

    public EntityValidationTests(
        ApplicationAssemblyContext assemblyContext,
        ITestOutputHelper output)
    {
        _entityTypeToConfigurationTypeMap = assemblyContext.EntityTypeToConfigurationTypeMap;
        _output = output;
    }

    public static IEnumerable<object[]> EntityTypesData
    {
        get
        {
            return new ApplicationAssemblyContext()
                 .EntityTypes
                 .Select(type => new object[] { type });
        }
    }

    [Theory]
    [MemberData(nameof(EntityTypesData))]
    public void EntityType_ShouldBe_AbstractOrSealed(Type entityType)
    {
        //Act
        var result = entityType.IsClass && (entityType.IsAbstract || entityType.IsSealed);

        // Assert
        result.Should()
            .BeTrue($"The entity {entityType.Name} is not abstract or sealed." +
                    "Entities should be sealed or abstract to prevent modification.");

        _output.WriteLine($"Entity {entityType.Name} is abstract or sealed");
    }

    public static IEnumerable<object[]> EntityTypeToConfigurationTypeMapData
    {
        get
        {
            return new ApplicationAssemblyContext()
                 .EntityTypeToConfigurationTypeMap
                 .Select(kvp => new object[] { kvp.Key, kvp.Value });
        }
    }

    [Theory]
    [MemberData(nameof(EntityTypeToConfigurationTypeMapData))]
    public void EntityType_ShouldHave_PublicProperties_WithPrivateOrProtectedSetters(
        Type entityType,
        Type entityConfigurationType)
    {
        // Arrange
        var conventionSet = new ConventionSet();
        var modelBuilder = new ModelBuilder(conventionSet);

        var entityConfigurationInstance = Activator.CreateInstance(entityConfigurationType);

        modelBuilder.ApplyConfiguration(entityConfigurationInstance as dynamic);

        var immutableEntityType = modelBuilder.Model.FindEntityType(entityType);

        Guard.NotNull(immutableEntityType);
         
        var entityBuilder = modelBuilder.Entity(entityType);

        foreach (var property in immutableEntityType.GetProperties())
        {
            if (property.PropertyInfo is null)
                continue;

            if (property.PropertyInfo.PropertyType == typeof(string))
            {
                var maxLength = property.GetMaxLength();

                maxLength.Should()
                    .NotBeNull($"Property {property.Name} of entity {entityType.Name} should have a max length defined in {entityConfigurationType.Name}");

                maxLength?.Should()
                    .BeGreaterThan(0, $"Property {property.Name} of entity {entityType.Name} should have a max length defined in {entityConfigurationType.Name}");

            }
        }
        _output.WriteLine($"Entity {entityType.Name}  has string properties with max length defined");
    }
}
