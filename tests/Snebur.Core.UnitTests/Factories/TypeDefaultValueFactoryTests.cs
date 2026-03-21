using Snebur.Core.Factories;

namespace Snebur.Core.UnitTests.Factories;

public class TypeDefaultValueFactoryTests
{
    [Theory]
    [InlineData(typeof(int), 0)]
    [InlineData(typeof(bool), false)]
    [InlineData(typeof(double), 0.0)]
    public void CreateDefaultValue_ValueTypes_ReturnsExpected(Type type, object expected)
    {
        var result = TypeDefaultValueFactory.CreateDefaultValue(type);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(typeof(string))]
    [InlineData(typeof(object))]
    [InlineData(typeof(List<int>))]
    public void CreateDefaultValue_ReferenceTypes_ReturnsNull(Type type)
    {
        var result = TypeDefaultValueFactory.CreateDefaultValue(type);
        result.Should().BeNull();
    }

    [Theory]
    [InlineData(typeof(int), 0)]
    [InlineData(typeof(bool), false)]
    [InlineData(typeof(string), "")]
    public void GetNotNullDefaultValue_PrimitiveTypes_ReturnsExpected(Type type, object expected)
    {
        var result = TypeDefaultValueFactory.GetNotNullDefaultValue(type);
        result.Should().Be(expected);
    }

    [Fact]
    public void GetNotNullDefaultValue_Array_ReturnsEmptyArray()
    {
        //Act
        var result = TypeDefaultValueFactory.GetNotNullDefaultValue(typeof(int[]));

        //Assert
        result.Should()
            .BeOfType<int[]>();

        (result as int[])
            .Should()
            .BeEmpty();
    }

    [Fact]
    public void GetNotNullDefaultValue_List_ReturnsEmptyList()
    {
        //Act
        var result = TypeDefaultValueFactory.GetNotNullDefaultValue(typeof(List<string>));

        //Assert
        result.Should()
            .BeOfType<List<string>>();

        (result as List<string>)
            .Should()
            .BeEmpty();
    }

    [Fact]
    public void GetNotNullDefaultValue_Dictionary_ReturnsEmptyDictionary()
    {
        //Act
        var result = TypeDefaultValueFactory.GetNotNullDefaultValue(typeof(Dictionary<string, int>));
         
        //Assert
        result.Should()
            .BeOfType<Dictionary<string, int>>();

        (result as Dictionary<string, int>)
            .Should()
            .BeEmpty();
    }

    [Fact]
    public void GetNotNullDefaultValue_HashSet_ReturnsEmptyHashSet()
    {
        //Act
        var result = TypeDefaultValueFactory.GetNotNullDefaultValue(typeof(HashSet<int>));

        //Assert
        result.Should()
            .BeOfType<HashSet<int>>();

        (result as HashSet<int>)
            .Should()
            .BeEmpty();
    }

    [Fact]
    public void GetNotNullDefaultValue_NonConcreteType_ThrowsInvalidCastException()
    {
        // Act
        Action act = () => TypeDefaultValueFactory.GetNotNullDefaultValue(typeof(IEnumerable<int>));

        // Assert
        act.Should()
            .Throw<InvalidCastException>()
           .WithMessage("*because it's not concrete.*");
    }
}
