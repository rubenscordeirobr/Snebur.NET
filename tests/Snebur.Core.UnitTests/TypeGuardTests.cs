namespace Snebur.Core.UnitTests;
public class TypeGuardTests
{
    [Theory]
    [InlineData(typeof(List<>))]
    [InlineData(typeof(Dictionary<,>))]
    public void MustBeNotGeneric_ShouldThrowException_WhenTypeIsGeneric(Type type)
    {
        Action act = () => TypeGuard.MustBeNotGeneric(type);
        act.Should().Throw<InvalidOperationException>().WithMessage("*is a generic type");
    }

    [Theory]
    [InlineData(typeof(string))]
    [InlineData(typeof(int))]
    public void MustBeNotGeneric_ShouldNotThrowException_WhenTypeIsNotGeneric(Type type)
    {
        Action act = () => TypeGuard.MustBeNotGeneric(type);
        act.Should().NotThrow();
    }

    [Theory]
    [InlineData(typeof(AbstractClass))]
    [InlineData(typeof(IInterface))]
    public void MustBeConcrete_ShouldThrowException_WhenTypeIsNotConcrete(Type type)
    {
        Action act = () => TypeGuard.MustBeConcrete(type);
        act.Should().Throw<InvalidOperationException>().WithMessage("*must be a concrete type");
    }

    [Theory]
    [InlineData(typeof(ConcreteClass))]
    public void MustBeConcrete_ShouldNotThrowException_WhenTypeIsConcrete(Type type)
    {
        Action act = () => TypeGuard.MustBeConcrete(type);
        act.Should().NotThrow();
    }

    [Theory]
    [InlineData(typeof(DerivedClass), typeof(BaseClass))]
    public void TypeMustBeAssignableFrom_ShouldNotThrowException_WhenTypeIsAssignableFromOther(Type type, Type other)
    {
        Action act = () => TypeGuard.TypeMustBeAssignableFrom(type, other);
        act.Should().NotThrow();
    }

    [Theory]
    [InlineData(typeof(BaseClass), typeof(DerivedClass))]
    public void TypeMustBeAssignableFrom_ShouldThrowException_WhenTypeIsNotAssignableFromOther(Type type, Type other)
    {
        Action act = () => TypeGuard.TypeMustBeAssignableFrom(type, other);
        act.Should().Throw<InvalidOperationException>().WithMessage("*must be a subclass of*");
    }

    [Theory]
    [InlineData(typeof(DerivedClass), typeof(BaseClass))]
    [InlineData(typeof(BaseClass), typeof(BaseClass))]
    public void TypeMustubclassOfOrEquals_ShouldNotThrowException_WhenTypeIsSubclassOfOrEquals(Type type, Type other)
    {
        Action act = () => TypeGuard.TypeMustBeSubclassOfOrEqual(type, other);
        act.Should().NotThrow();
    }

    [Theory]
    [InlineData(typeof(BaseClass), typeof(DerivedClass))]
    public void TypeMustubclassOfOrEquals_ShouldThrowException_WhenTypeIsNotSubclassOfOrEquals(Type type, Type other)
    {
        Action act = () => TypeGuard.TypeMustBeSubclassOfOrEqual(type, other);
        act.Should().Throw<InvalidOperationException>().WithMessage("*must be a subclass of*");
    }

    [Theory]
    [InlineData(typeof(IInterface))]
    public void TypeMustBeInterface_ShouldNotThrowException_WhenTypeIsInterface(Type type)
    {
        Action act = () => TypeGuard.TypeMustBeInterface(type, type);
        act.Should().NotThrow();
    }

    [Theory]
    [InlineData(typeof(ConcreteClass))]
    public void TypeMustBeInterface_ShouldThrowException_WhenTypeIsNotInterface(Type type)
    {
        Action act = () => TypeGuard.TypeMustBeInterface(type, type);
        act.Should().Throw<InvalidOperationException>().WithMessage("*must be an interface");
    }

    [Theory]
    [InlineData(typeof(ConcreteClass), typeof(IInterface))]
    public void TypeMustImplementInterface_ShouldNotThrowException_WhenTypeImplementsInterface(Type type, Type interfaceType)
    {
        Action act = () => TypeGuard.TypeMustImplementInterface(type, interfaceType);
        act.Should().NotThrow();
    }

    [Theory]
    [InlineData(typeof(ConcreteClass), typeof(IOtherInterface))]
    public void TypeMustImplementInterface_ShouldThrowException_WhenTypeDoesNotImplementInterface(Type type, Type interfaceType)
    {
        Action act = () => TypeGuard.TypeMustImplementInterface(type, interfaceType);
        act.Should().Throw<InvalidOperationException>().WithMessage("*must implement*");
    }

    private abstract class AbstractClass { }
    private interface IInterface { }
    private interface IOtherInterface { }
    private class BaseClass { }
    private class DerivedClass : BaseClass { }
    private class ConcreteClass : IInterface { }
}
