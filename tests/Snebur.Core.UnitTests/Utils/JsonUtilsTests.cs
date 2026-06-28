using System.Text.Json;
using Snebur.Core.Helpers;

namespace Snebur.Core.UnitTests.Utils;

public class JsonUtilsTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    private static readonly object _lock = new();
    public JsonUtilsTests(ITestOutputHelper testOutput)
    {
        _testOutputHelper = testOutput;
    }

    [Fact]
    public void Serialize_ShouldReturnJsonString_WhenObjectIsValid()
    {
        // Arrange
        var obj = new TestObject("Alice", 30);

        // Act
        var json = JsonUtils.Serialize(obj);

        // Assert
        json.Should().Contain("Alice").And.Contain("30");
    }

    [Fact]
    public void Deserialize_Generic_ShouldReturnValidObject_WhenJsonIsValid()
    {
        // Arrange
        var json = "{\"Name\":\"Bob\",\"Age\":25}";

        // Act
        var result = JsonUtils.Deserialize<TestObject>(json);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Bob");
        result.Age.Should().Be(25);
    }

    [Fact]
    public void Deserialize_NonGeneric_ShouldReturnValidObject_WhenJsonIsValid()
    {
        // Arrange
        var json = "{\"Name\":\"Charlie\",\"Age\":40}";

        // Act
        var result = JsonUtils.Deserialize(json, typeof(TestObject));

        // Assert
        result.Should().NotBeNull().And.BeOfType<TestObject>();

        var typedResult = result as TestObject;
        typedResult!.Name.Should().Be("Charlie");
        typedResult.Age.Should().Be(40);
    }

    [Fact]
    public void Serialize_ShouldReturnJsonNull_WhenObjectIsNull()
    {
        // Arrange
        TestObject? obj = null;

        // Act
        var json = JsonUtils.Serialize(obj);

        // Assert
        json.Should().Be("null");
    }

    [Fact]
    public void Deserialize_ShouldReturnNull_WhenJsonIsNull()
    {
        // Arrange
        var json = "null";

        // Act
        var result = JsonUtils.Deserialize<TestObject>(json);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Deserialize_ShouldThrowJsonException_WhenJsonIsInvalid()
    {
        // Arrange
        var invalidJson = "invalid json";

        // Act
        Action act = () => JsonUtils.Deserialize<TestObject>(invalidJson);

        // Assert
        act.Should().Throw<JsonException>();
    }

    //[Fact]
    public void EnableIndentationInDevelopment_ShouldEnableIndentation_WhenEnvironmentIsDevelopment_AndWriteIndentedIsFalse()
    {
        lock (_lock)
        {
            // Arrange
            var originalEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            try
            {
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

                _testOutputHelper.WriteLine($"Original Environment: {originalEnvironment}." +
                        $" Current {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}" +
                        $" {EnvironmentHelper.IsDevelopment()}");

                var options = new JsonSerializerOptions
                {
                    WriteIndented = false
                };

                _testOutputHelper.WriteLine($"Original Environment: {EnvironmentHelper.IsDevelopment()}");
                EnvironmentHelper.Reset();
                // Act
                JsonUtils.EnableIndentationInDevelopment(options);

                // Assert
                options.WriteIndented
                    .Should()
                    .BeTrue();
            }
            finally
            {
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", originalEnvironment);
                EnvironmentHelper.Reset();
            }
        }
    }

    //[Fact]
    public static void EnableIndentationInDevelopment_ShouldNotChangeIndentation_WhenWriteIndentedIsAlreadyTrue()
    {
        lock (_lock)
        {
      
            // Arrange
            var originalEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            try
            {
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
                Environment.SetEnvironmentVariable("XUNIT_ENVIRONMENT", null);

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                EnvironmentHelper.Reset();
                // Act
                JsonUtils.EnableIndentationInDevelopment(options);

                // Assert
                options.WriteIndented.Should().BeTrue();
            }
            finally
            {
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", originalEnvironment);
                Environment.SetEnvironmentVariable("XUNIT_ENVIRONMENT", "TEST");
                EnvironmentHelper.Reset();
            }
        }
    }

    //[Fact]
    public static void EnableIndentationInDevelopment_ShouldNotEnableIndentation_WhenEnvironmentIsNotDevelopment()
    {
        lock (_lock)
        {
            EnvironmentHelper.Reset();
            // Arrange
            var originalEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            try
            {
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Production");
                Environment.SetEnvironmentVariable("XUNIT_ENVIRONMENT", null);
                var options = new JsonSerializerOptions
                {
                    WriteIndented = false
                };

                // Act
                JsonUtils.EnableIndentationInDevelopment(options);

                // Assert
                options.WriteIndented.Should().BeFalse();
            }
            finally
            {
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", originalEnvironment);
                Environment.SetEnvironmentVariable("XUNIT_ENVIRONMENT", "TEST");
            }
        }
    }

    [Fact]
    public static void EnableIndentationInDevelopment_ShouldThrowArgumentNullException_WhenOptionsIsNull()
    {
        // Arrange
        Action act = () => JsonUtils.EnableIndentationInDevelopment(null!);

        // Act & Assert
        act.Should().Throw<ArgumentNullException>();
    }

    public static IEnumerable<object[]> GetJsonSchemaTypeTestData()
    {
        yield return new object[] { typeof(string), "string" };
        yield return new object[] { typeof(bool), "boolean" };

        // Integer types
        yield return new object[] { typeof(int), "integer" };
        yield return new object[] { typeof(long), "integer" };
        yield return new object[] { typeof(short), "integer" };
        yield return new object[] { typeof(byte), "integer" };
        yield return new object[] { typeof(uint), "integer" };
        yield return new object[] { typeof(ulong), "integer" };
        yield return new object[] { typeof(ushort), "integer" };

        // Number types
        yield return new object[] { typeof(float), "number" };
        yield return new object[] { typeof(double), "number" };
        yield return new object[] { typeof(decimal), "number" };

        // Enum type
        yield return new object[] { typeof(TestEnum), "number" };

        // Enumerable type (excluding string)
        yield return new object[] { typeof(int[]), "array" };

        // Nullable types
        yield return new object[] { typeof(int?), "integer" };
        yield return new object[] { typeof(bool?), "boolean" };

        // Default object type
        yield return new object[] { typeof(JsonUtilsTests), "object" };
    }

    [Theory]
    [MemberData(nameof(GetJsonSchemaTypeTestData))]
    public void GetJsonSchemaType_ShouldReturnExpectedType(Type inputType, string expectedSchemaType)
    {
        // Act
        var schemaType = JsonUtils.GetJsonSchemaType(inputType);

        // Assert
        schemaType.Should().Be(expectedSchemaType);
    }

    private record TestObject(string Name, int Age);

    private enum TestEnum
    {
        First,
        Second,
        Third
    }
}

