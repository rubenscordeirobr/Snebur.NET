using System.ComponentModel.DataAnnotations.Schema;

namespace Snebur.Core.UnitTests.Extensions;

public class TypeExtensionsTests
{
    public static IEnumerable<object[]> IsSubclassOfTestData
        => new List<object[]>
        {
            new object[] { typeof(DerivedClass), typeof(BaseClass), true },
            new object[] { typeof(BaseClass), typeof(DerivedClass), false },
            new object[] { typeof(object), typeof(BaseClass), false },
            new object[] { typeof(DerivedClass), typeof(object), true },
        };

    [Theory]
    [MemberData(nameof(IsSubclassOfTestData))]
    public void IsSubclassOf_ShouldReturnExpectedResult(
        Type type,
        Type baseType,
        bool expectedResult)
    {
        type.IsSubclassOf(baseType).Should().Be(expectedResult);
    }

    public static IEnumerable<object[]> IsSubclassOfOrEqualsTestData
        => new List<object[]>
        {
            new object[] { typeof(DerivedClass), typeof(BaseClass), true },
            new object[] { typeof(BaseClass), typeof(DerivedClass), false },
            new object[] { typeof(BaseClass), typeof(BaseClass), true },
            new object[] { typeof(object), typeof(BaseClass), false },
            new object[] { typeof(DerivedClass), typeof(object), true },
        };

    [Theory]
    [MemberData(nameof(IsSubclassOfOrEqualsTestData))]
    public void IsSubclassOfOrEquals_ShouldReturnExpectedResult(
        Type type,
        Type otherType,
        bool expectedResult)
    {
        type.IsSubclassOfOrEquals(otherType).Should().Be(expectedResult);
    }

    public static IEnumerable<object[]> IsImplementsGenericInterfaceDefinitionData
        => new List<object[]>
        {
            new object[] { typeof(List<string>), typeof(IEnumerable<>), true },
            new object[] { typeof(List<string>), typeof(IList<>), true },
            new object[] { typeof(List<string>), typeof(ICollection<>), true },
            new object[] { typeof(string), typeof(IEnumerable<>), true },
            new object[] { typeof(DerivedClass), typeof(IEnumerable<>), false },
        };

    [Theory]
    [MemberData(nameof(IsImplementsGenericInterfaceDefinitionData))]
    public void IsImplementsGenericInterfaceDefinition_ShouldReturnExpectedResult(
        Type givenType,
        Type genericType,
        bool expectedResult)
    {
        givenType.ImplementsGenericInterfaceDefinition(genericType).Should().Be(expectedResult);
    }

    public static IEnumerable<object[]> GetDeclaredPropertiesTestData
        => new List<object[]>
        {
            new object[] { typeof(TestClass), true, new[] { "Property1", "Property2" } },
            new object[] { typeof(TestClass), false, new[] { "Property1", "Property2", "NotMappedProperty" } },
        };

    [Theory]
    [MemberData(nameof(GetDeclaredPropertiesTestData))]
    public void GetDeclaredProperties_ShouldReturnExpectedResult(
        Type type,
        bool isIgnoreNotMappedAtribute,
        string[] expectedProperties)
    {
        var properties = type.GetDeclaredProperties(isIgnoreNotMappedAtribute)
            .Select(p => p.Name)
            .ToArray();

        properties.Should().BeEquivalentTo(expectedProperties);
    }
     
    public static IEnumerable<object[]> GetAssignableTypesTestData
        => new List<object[]>
        {
            new object[] { typeof(DerivedClass), new[] { typeof(DerivedClass), typeof(BaseClass) } },
            new object[] { typeof(BaseClass), new[] { typeof(BaseClass) } },
            new object[] { typeof(List<string>), new[] { typeof(List<string>), typeof(IEnumerable<string>), typeof(ICollection<string>), typeof(IList<string>) } },
        };

    [Theory]
    [MemberData(nameof(GetAssignableTypesTestData))]
    public void GetAssignableTypes_ShouldReturnExpectedResult(Type type, Type[] expectedTypes)
    {
        var result = type.GetAssignableTypes().ToArray();
        result.Should().Contain(expectedTypes);
    }

    public static IEnumerable<object[]> GetQualifiedTypeNameTestData
        => new List<object[]>
        {
            new object[] { typeof(string), "System.String" },
            new object[] { typeof(List<string>), "List<System.String>" },
            new object[] { typeof(Dictionary<int, string>), "Dictionary<System.Int32, System.String>" },
        };
     
    [Theory]
    [MemberData(nameof(GetQualifiedTypeNameTestData))]
    public void GetQualifiedName_ShouldReturnExpectedResult(Type type, string expectedName)
    {
        var result = type.GetQualifiedName();
        result.Should().Be(expectedName);
    }

    public static IEnumerable<object[]> GetPropertiesFromInterfaceTestData
       => new List<object[]>
       {
            new object[] { typeof(TestClassWithInterface), typeof(ITestInterface), new[] { "Property1", "Property2" } },
       };
     
    [Theory]
    [MemberData(nameof(GetPropertiesFromInterfaceTestData))]
    public void GetPropertiesFromInterface_ShouldReturnExpectedResult(Type type, Type interfaceType, string[] expectedProperties)
    {
        var result = type.GetPropertiesFromInterface(interfaceType).Keys.ToArray();
        result.Should().BeEquivalentTo(expectedProperties);
    }

    public static IEnumerable<object[]> GetDeclaredPropertiesOfTypeTestData
        => new List<object[]>
        {
            new object[] { typeof(TestClass), typeof(string), true, new[] { "Property1" } },
            new object[] { typeof(TestClass), typeof(int), true, new[] { "Property2" } },
            new object[] { typeof(TestClass), typeof(string), false, new[] { "Property1", "NotMappedProperty" } },
        };

    [Theory]
    [MemberData(nameof(GetDeclaredPropertiesOfTypeTestData))]
    public void GetDeclaredPropertiesOfType_ShouldReturnExpectedResult(
        Type type,
        Type propertyType,
        bool isIgnoreNotMappedAtribute,
        string[] expectedProperties)
    {
        var properties = type.GetDeclaredPropertiesOfType(propertyType, isIgnoreNotMappedAtribute)
            .Select(p => p.Name)
            .ToArray();

        properties.Should().BeEquivalentTo(expectedProperties);
    }

    public static IEnumerable<object[]> GetSingleNameTestData
      => new List<object[]>
      {
            new object[] { typeof(List<string>), "List" },
            new object[] { typeof(Dictionary<int, string>), "Dictionary" },
            new object[] { typeof(string), "String" },
      };

    [Theory]
    [MemberData(nameof(GetSingleNameTestData))]
    public void GetSingleName_ShouldReturnExpectedResult(Type type, string expectedName)
    {
        var result = type.GetSingleName();
        result.Should().Be(expectedName);
    }

    public static IEnumerable<object[]> IsAssignableToTestData
       => new List<object[]>
       {
            new object[] { typeof(List<string>), new[] { typeof(IEnumerable<string>), typeof(ICollection<string>) }, true },
            new object[] { typeof(string), new[] { typeof(IEnumerable<char>), typeof(IComparable) }, true },
            new object[] { typeof(int), new[] { typeof(IComparable), typeof(IConvertible) }, true },
            new object[] { typeof(DerivedClass), new[] { typeof(BaseClass), typeof(object) }, true },
            new object[] { typeof(BaseClass), new[] { typeof(DerivedClass) }, false },
       };

    [Theory]
    [MemberData(nameof(IsAssignableToTestData))]
    public void IsAssignableTo_ShouldReturnExpectedResult(Type type, Type[] targetTypes, bool expectedResult)
    {
        var result = type.IsAssignableTo(targetTypes);
        result.Should().Be(expectedResult);
    }
     
    public static IEnumerable<object[]> IsAssignableToOrDefinitionTestData
        => new List<object[]>
        {
            new object[] { typeof(List<string>), new[] { typeof(IEnumerable<string>), typeof(IEnumerable<>), typeof(ICollection<>) }, true },
            new object[] { typeof(string), new[] { typeof(IEnumerable<>), typeof(IComparable) }, true },
            new object[] { typeof(int), new[] { typeof(IComparable), typeof(IConvertible) }, true },
            new object[] { typeof(DerivedClass), new[] { typeof(BaseClass), typeof(object) }, true },
            new object[] { typeof(BaseClass), new[] { typeof(DerivedClass) }, false },
        };
     
    [Theory]
    [MemberData(nameof(IsAssignableToOrDefinitionTestData))]
    public void IsAssignableToOrDefinition_ShouldReturnExpectedResult(Type type, Type[] targetTypes, bool expectedResult)
    {
        var result = type.IsAssignableToOrDefinition(targetTypes);
        result.Should().Be(expectedResult);
    }

    public static IEnumerable<object[]> GetUnderlyingTypeTestData
    => new List<object[]>
    {
            new object[] { typeof(int?), typeof(int) },
            new object[] { typeof(string), typeof(string) },
            new object[] { typeof(Nullable<>), typeof(Nullable<>) },
            new object[] { typeof(List<string>), typeof(List<string>) },
    };

    [Theory]
    [MemberData(nameof(GetUnderlyingTypeTestData))]
    public void GetUnderlyingType_ShouldReturnExpectedResult(Type type, Type expectedType)
    {
        var result = type.GetUnderlyingType();
        result.Should().Be(expectedType);
    }

    [Fact]
    public void GetPropertyByName_ShouldReturnPropertyInfo_WhenPropertyExists()
    {
        // Arrange
        var type = typeof(DummyClass);

        // Act
        var propertyInfo = type.GetPropertyByName("Name");

        // Assert
        propertyInfo.Should().NotBeNull();
        propertyInfo!.Name.Should().Be("Name");
    }

    [Fact]
    public void GetPropertyByName_ShouldReturnPropertyInfo_IgnoringCase()
    {
        // Arrange
        var type = typeof(DummyClass);

        // Act
        var propertyInfo = type.GetPropertyByName("name");

        // Assert
        propertyInfo.Should().NotBeNull();
        propertyInfo!.Name.Should().Be("Name");
    }

    [Fact]
    public void GetPropertyByName_ShouldReturnNull_WhenPropertyDoesNotExist()
    {
        // Arrange
        var type = typeof(DummyClass);

        // Act
        var propertyInfo = type.GetPropertyByName("NonExistent");

        // Assert
        propertyInfo.Should().BeNull();
    }

    [Fact]
    public void GetRequiredProperty_ShouldReturnPropertyInfo_WhenPropertyExists()
    {
        // Arrange
        var type = typeof(DummyClass);

        // Act
        var propertyInfo = type.GetRequiredProperty("Age");

        // Assert
        propertyInfo.Should().NotBeNull();
        propertyInfo.Name.Should().Be("Age");
    }

    [Fact]
    public void GetRequiredProperty_ShouldThrowInvalidOperationException_WhenPropertyDoesNotExist()
    {
        // Arrange
        var type = typeof(DummyClass);

        // Act
        Action act = () => type.GetRequiredProperty("NonExistent");

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage($"The property NonExistent was not found in the type {type.Name}");
    }

    [Fact]
    public void GetDisplayName_ShouldThrowArgumentNullException_WhenTypeIsNull()
    {
        // Arrange
        Type? type = null;

        // Act
        Action act = () => type!.GetDisplayName();

        // Assert
        act.Should().Throw<ArgumentNullException>("because the guard method enforces a non-null type");
    }

    [Fact]
    public void GetDisplayName_ShouldReturnTypeName_ForNonGenericType()
    {
        // Arrange
        Type type = typeof(string);

        // Act
        string displayName = type.GetDisplayName();

        // Assert
        displayName.Should().Be("String", "because for a non-generic type, the display name is its Name");
    }

    [Fact]
    public void GetDisplayName_ShouldReturnGenericTypeName_WithGenericArguments()
    {
        // Arrange
        Type type = typeof(List<int>);

        // Act
        string displayName = type.GetDisplayName();

        // Assert
        displayName.Should().Be("List<Int32>", "because the generic type List<int> should be displayed with its type argument");
    }

    [Fact]
    public void GetDisplayName_ShouldReturnGenericTypeName_WithMultipleGenericArguments()
    {
        // Arrange
        Type type = typeof(Dictionary<string, int>);

        // Act
        string displayName = type.GetDisplayName();

        // Assert
        displayName.Should().Be("Dictionary<String, Int32>", "because Dictionary<string, int> should list both type arguments");
    }

    [Fact]
    public void GetDisplayName_ShouldReturnNestedTypeName_WithDeclaringTypeName()
    {
        // Arrange
        Type type = typeof(Outer.Nested);

        // Act
        string displayName = type.GetDisplayName();

        // Assert
        displayName.Should().Be("Outer.Nested", "because nested types should include the declaring type name");
    }

    // Dummy types for testing nested types.
    private class Outer
    {
        public class Nested { }
    }

    private class BaseClass { }
    private class DerivedClass : BaseClass { }

    private class TestClass
    {
        public string? Property1 { get; set; }
        public int Property2 { get; set; }

        [NotMapped]
        public string? NotMappedProperty { get; set; }
    }

    private interface ITestInterface
    {
        string? Property1 { get; set; }
        int Property2 { get; set; }
    }

    private class TestClassWithInterface : ITestInterface
    {
        public string? Property1 { get; set; }
        public int Property2 { get; set; }
        public string? Property3 { get; set; }
    }

    // Dummy class for testing purposes
    private class DummyClass
    {
        public string? Name { get; set; }
        public int Age { get; set; }
    }
}
