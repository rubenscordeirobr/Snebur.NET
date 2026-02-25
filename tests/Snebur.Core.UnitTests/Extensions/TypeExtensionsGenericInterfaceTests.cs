namespace Snebur.Core.UnitTests.Extensions;

public class TypeExtensionsGenericInterfaceTests
{
    private interface IGenericInterface<T> { }

    private class GenericImpl : IGenericInterface<int> { }

    private class NonImplementingClass { }

    [Fact]
    public void GetGenericArgumentFromInterfaceDefinition_ShouldReturnExpectedArgument()
    {
        // Arrange
        var type = typeof(GenericImpl);

        // Act
        var result = type.GetGenericArgumentFromInterfaceDefinition(typeof(IGenericInterface<>));

        // Assert
        result.Should().Be(typeof(int));
    }

    [Fact]
    public void GetGenericArgumentsFromInterfaceDefinition_ShouldReturnExpectedArguments()
    {
        // Arrange
        var type = typeof(GenericImpl);

        // Act
        var result = type.GetGenericArgumentsFromInterfaceDefinition(typeof(IGenericInterface<>));

        // Assert
        result.Should().BeEquivalentTo(new[] { typeof(int) });
    }

    [Fact]
    public void GetGenericArgumentsFromInterfaceDefinition_WithNonGenericTypeDefinition_ShouldThrowArgumentException()
    {
        // Arrange
        var type = typeof(GenericImpl);

        // Act
        Action act = () => type.GetGenericArgumentsFromInterfaceDefinition(typeof(IGenericInterface<int>));

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("The interface definition must be a generic type definition*");
    }

    [Fact]
    public void GetGenericArgumentsFromInterfaceDefinition_WhenTypeDoesNotImplementInterface_ShouldThrowArgumentException()
    {
        // Arrange
        var type = typeof(NonImplementingClass);

        // Act
        Action act = () => type.GetGenericArgumentsFromInterfaceDefinition(typeof(IGenericInterface<>));

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("The type * does not implement the interface IGenericInterface*");
    }
}
