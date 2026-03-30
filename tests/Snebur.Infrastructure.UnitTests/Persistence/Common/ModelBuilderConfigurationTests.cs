using Snebur.SharedKernel.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Snebur.Infrastructure.UnitTests.Persistence.Common;

public class ModelBuilderConfigurationTests
{

    [Fact]
    public void EntityBuilderConfiguration_ShouldApplyDefaultConfiguration()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        // Act
        using var context = new TestDbContext(options);
        var testEntityType = context.Model.FindEntityType(typeof(TestEntity));
        var relationTestEntityType = context.Model.FindEntityType(typeof(RelationTestEntity));  

        Guard.NotNull(testEntityType);

        var idProperty = testEntityType.FindProperty(nameof(EntityBase.Id))!;
        var createdAtProperty = testEntityType.FindProperty(nameof(EntityBase.CreatedAt))!;
        var lastUpdatedAtProperty = testEntityType.FindProperty(nameof(EntityBase.LastUpdatedAt))!;
        var emailProperty = testEntityType.FindProperty(nameof(TestEntity.Email))!;
        var sortOrderProperty = testEntityType.FindProperty(nameof(TestEntity.SortOrder))!;
        var deletedSessionIdProperty = testEntityType.FindProperty(nameof(ISoftDeletableEntity.DeletedSession_Id))!;
       
        var deletedSessionIdValueConverter = deletedSessionIdProperty?.GetValueConverter()!;
        var emailIndex = testEntityType.FindIndex(emailProperty)!;
         
        var testEntityIdProperty = relationTestEntityType!.FindProperty(nameof(RelationTestEntity.TestEntity_Id))!;
        var foreignKey = relationTestEntityType!.GetForeignKeys()
            .FirstOrDefault()!;

        var testEntityIdValueConverter = testEntityIdProperty?.GetValueConverter()!;

        // Assert
        IProperty[] properties = [
            idProperty,
            createdAtProperty,
            lastUpdatedAtProperty,
            emailProperty,
            sortOrderProperty,
            deletedSessionIdProperty!
        ];

        properties.Should()
            .NotContainNulls();

        idProperty.ValueGenerated
            .Should()
            .Be(ValueGenerated.OnAdd);

        idProperty.GetDefaultValueSql()
            .Should()
            .Be("uuid_generate_v4()");

        createdAtProperty.GetDefaultValueSql()
            .Should()
            .Be("now()");

        lastUpdatedAtProperty.GetDefaultValueSql()
            .Should()
            .Be("now()");
         
        emailIndex.Should()
            .NotBeNull();

        emailIndex.IsUnique
            .Should()
            .BeTrue();

        emailIndex.GetFilter()
            .Should()
            .Be("is_deleted = false");

        sortOrderProperty.GetDefaultValueSql()
            .Should()
            .Be("get_next_sort_order_asc('test_entity', 'sort_order')");
         
        deletedSessionIdValueConverter
            .Should()
            .NotBeNull();

        deletedSessionIdValueConverter
            .Should()
            .BeOfType<NullableGuidValueConverter>();

        deletedSessionIdProperty!.GetColumnName()
            .Should()
            .Be("deleted_session_id");

        foreignKey.Should()
            .NotBeNull();

        foreignKey.DeleteBehavior
            .Should()
            .Be(DeleteBehavior.Restrict);
         
        testEntityIdValueConverter.Should()
            .NotBeNull();

        testEntityIdValueConverter.Should()
            .BeOfType<NotEmptyGuidValidation>();

    }
    
    public class TestDbContext : DbContext
    {
        public DbSet<TestEntity> TestEntities { get; set; }

        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ConfigureModelDefaultConfiguration<TestDbContext>(isInMemory: false);
            base.OnModelCreating(modelBuilder);
        }
    }

    public class TestEntity : EntityBase, ISoftDeletableEntity, ISortable
    {
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedSession_Id { get; set; }
        public double? SortOrder { get; set; }
        public required string Email { get; set; }
        public List<RelationTestEntity> Relations { get; } = new();
    }

    public class RelationTestEntity : EntityBase
    {
        public required string Name { get; set; }
        public TestEntity? TestEntity { get; set; }
        public Guid TestEntity_Id { get; set; }
    }

    public class TestEntityConfiguration : IEntityTypeConfiguration<TestEntity>
    {
        public void Configure(EntityTypeBuilder<TestEntity> builder)
        {
            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(x => x.Email)
                .IsUnique();
        }
    }

    public class RelationTestEntityConfiguration : IEntityTypeConfiguration<RelationTestEntity>
    {
        public void Configure(EntityTypeBuilder<RelationTestEntity> builder)
        {
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasOne(x => x.TestEntity)
                .WithMany(x => x.Relations)
                .HasForeignKey(x => x.TestEntity_Id);
        }
    }
}
