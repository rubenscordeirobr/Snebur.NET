using Snebur.Domain.Helpers;
using Snebur.SharedKernel.Abstractions;

namespace Snebur.Application.UnitTests.Domain.Helpers;

public class EntityReflectionHelperTests
{
    [Fact]
    public void IsImplementDeletedInterface_Generic_ShouldReturnTrue_ForEntityImplementingInterface()
    {
        // Act
        bool result = EntityReflectionHelper.IsImplementDeletedInterface<TestSoftDeletableEntity>();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsImplementDeletedInterface_Generic_ShouldReturnFalse_ForEntityNotImplementingInterface()
    {
        // Act
        bool result = EntityReflectionHelper.IsImplementDeletedInterface<TestNonImplementedEntity>();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsImplementTenantOwnedInterface_Generic_ShouldReturnTrue_ForEntityImplementingInterface()
    {
        // Act
        bool result = EntityReflectionHelper.IsImplementTenantOwnedInterface<TestTenantOwnedEntity>();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsImplementTenantOwnedInterface_Generic_ShouldReturnFalse_ForEntityNotImplementingInterface()
    {
        // Act
        bool result = EntityReflectionHelper.IsImplementTenantOwnedInterface<TestNonImplementedEntity>();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsImplementDeletedInterface_NonGeneric_ShouldReturnTrue_ForEntityImplementingInterface()
    {
        // Act
        bool result = EntityReflectionHelper.IsImplementDeletedInterface(typeof(TestSoftDeletableEntity));

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsImplementDeletedInterface_NonGeneric_ShouldReturnFalse_ForEntityNotImplementingInterface()
    {
        // Act
        bool result = EntityReflectionHelper.IsImplementDeletedInterface(typeof(TestNonImplementedEntity));

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsImplementTenantOwnedInterface_NonGeneric_ShouldReturnTrue_ForEntityImplementingInterface()
    {
        // Act
        bool result = EntityReflectionHelper.IsImplementTenantOwnedInterface(typeof(TestTenantOwnedEntity));

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsImplementTenantOwnedInterface_NonGeneric_ShouldReturnFalse_ForEntityNotImplementingInterface()
    {
        // Act
        bool result = EntityReflectionHelper.IsImplementTenantOwnedInterface(typeof(TestNonImplementedEntity));

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsImplementDeletedInterface_ShouldThrowException_WhenEntityTypeIsNull()
    {
        // Act
        Action act = () => EntityReflectionHelper.IsImplementDeletedInterface(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void IsImplementTenantOwnedInterface_ShouldThrowException_WhenEntityTypeIsNull()
    {
        // Act
        Action act = () => EntityReflectionHelper.IsImplementTenantOwnedInterface(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    public class TestSoftDeletableEntity : EntityBase, ISoftDeletableEntity
    {
        public bool IsDeleted { get; }

        public DateTime? DeletedAt { get; }

        public Guid? DeletedSession_Id { get; }
    }

    public class TestTenantOwnedEntity : EntityBase, ITenantOwned
    {
        public Guid Tenant_Id { get; }
    }

    public class TestNonImplementedEntity : EntityBase { }
}
