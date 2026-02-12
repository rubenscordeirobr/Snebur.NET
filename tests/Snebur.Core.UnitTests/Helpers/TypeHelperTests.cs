using Snebur.Core.Helpers;

namespace Snebur.Core.UnitTests.Helpers;

public class TypeHelperTests
{
    public static IEnumerable<object[]> GetTestData()
    {
        yield return new object[]
        {
            new Type[] { typeof(TestHandler) },
            typeof(IHandler<>),
            true,
            new List<(Type, Type)> { (typeof(TestHandler), typeof(IHandler<TestMessage>)) }
        };

        yield return new object[]
        {
            new Type[] { typeof(TestHandler), typeof(NonConcreteHandler) },
            typeof(IHandler<>),
            true,
            new List<(Type, Type)> { (typeof(TestHandler), typeof(IHandler<TestMessage>)) }
        };

        yield return new object[]
        {
            new Type[] { typeof(TestHandler), typeof(NonConcreteHandler) },
            typeof(IHandler<>),
            false,
            new List<(Type, Type)>
            {
                (typeof(TestHandler), typeof(IHandler<TestMessage>)),
                (typeof(NonConcreteHandler), typeof(IHandler<TestMessage>))
            }
        };
    }

    [Theory]
    [MemberData(nameof(GetTestData))]
    public void MapWithInterfaces_ShouldReturnExpectedResults(
        Type[] types,
        Type interfaceDefinitionType,
        bool isConcrete,
        List<(Type, Type)> expected)
    {
        // Act
        var result = TypeHelper.FindTypesImplementingInterface(types, interfaceDefinitionType, isConcrete);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
     
    // Test classes and interfaces
    public interface IHandler<T> { }

    public class TestMessage { }

    public class TestHandler : IHandler<TestMessage> { }

    public abstract class NonConcreteHandler : IHandler<TestMessage> { }

    public class InvalidHandler { }
}
