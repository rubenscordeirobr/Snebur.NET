using System.Reflection;
using Snebur.Core.Exceptions;

namespace Snebur.Core.UnitTests.Extensions;

public class ReflectionNullabilityExtensionsTests
{
    #region IsNullable(PropertyInfo)

    [Theory]
    [InlineData(nameof(TestClass.NonNullableReferenceProperty), false)]
    [InlineData(nameof(TestClass.NullableReferenceProperty), true)]
    [InlineData(nameof(TestClass.ValueTypeProperty), false)]
    [InlineData(nameof(TestClass.NullableValueTypeProperty), false)] // Value types return false regardless of nullability
    public void IsNullable_PropertyInfo_ShouldReturnCorrectValue(string propertyName, bool expected)
    {
        // Arrange
        var property = typeof(TestClass).GetProperty(propertyName);

        // Act
        var result = property?.IsNullable();

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void IsNullable_PropertyInfo_WithNullProperty_ShouldThrowArgumentNullException()
    {
        // Arrange
        PropertyInfo? property = null;

        // Act & Assert
        var action = () => property!.IsNullable();
        action.Should().Throw<ArgumentNullException>()
              .WithParameterName("property");
    }

    #endregion

    #region IsNullable(ParameterInfo)

    [Theory]
    [InlineData(nameof(TestClass.MethodWithNonNullableParameter), "nonNullableParam", false)]
    [InlineData(nameof(TestClass.MethodWithNullableParameter), "nullableParam", true)]
    [InlineData(nameof(TestClass.MethodWithValueTypeParameter), "valueTypeParam", false)]
    [InlineData(nameof(TestClass.MethodWithNullableValueTypeParameter), "nullableValueTypeParam", false)] // Value types return false regardless of nullability
    public void IsNullable_ParameterInfo_ShouldReturnCorrectValue(string methodName, string paramName, bool expected)
    {
        // Arrange
        var method = typeof(TestClass).GetMethod(methodName);
        var parameter = method?.GetParameters().First(p => p.Name == paramName);

        // Act
        var result = parameter!.IsNullable();

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void IsNullable_ParameterInfo_WithNullParameter_ShouldThrowArgumentNullException()
    {
        // Arrange
        ParameterInfo? parameter = null;

        // Act & Assert
        var action = () => parameter!.IsNullable();
        action.Should().Throw<ArgumentNullException>()
              .WithParameterName("parameter");
    }

    #endregion

    #region IsNullable(FieldInfo)

    [Theory]
    [InlineData(nameof(TestClass.nonNullableField), false)]
    [InlineData(nameof(TestClass.nullableField), true)]
    [InlineData(nameof(TestClass.valueTypeField), false)]
    [InlineData(nameof(TestClass.nullableValueTypeField), false)] // Value types return false regardless of nullability
    public void IsNullable_FieldInfo_ShouldReturnCorrectValue(string fieldName, bool expected)
    {
        // Arrange
        var field = typeof(TestClass).GetField(fieldName);

        // Act
        var result = ReflectionNullabilityExtensions.IsNullable(field!);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void IsNullable_FieldInfo_WithNullField_ShouldThrowArgumentNullException()
    {
        // Arrange
        FieldInfo? field = null;

        // Act & Assert
        var action = () => ReflectionNullabilityExtensions.IsNullable(field!);
        action.Should().Throw<ArgumentNullException>()
              .WithParameterName("field");
    }

    #endregion

    #region IsNullable(EventInfo)

    [Theory]
    [InlineData(nameof(TestClass.NonNullableEvent), false)]
    [InlineData(nameof(TestClass.NullableEvent), true)]
    public void IsNullable_EventInfo_ShouldReturnCorrectValue(string eventName, bool expected)
    {
        // Arrange
        var eventInfo = typeof(TestClass).GetEvent(eventName);

        // Act
        var result = eventInfo?.IsNullable();

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void IsNullable_EventInfo_WithNullEvent_ShouldThrowArgumentNullException()
    {
        // Arrange
        EventInfo? eventInfo = null;

        // Act & Assert
        var action = () => eventInfo!.IsNullable();
        action.Should().Throw<ArgumentNullException>()
              .WithParameterName("eventInfo");
    }

    #endregion

    #region TestClass

#pragma warning disable S3459, S1186, CS0649, CS0067

    private class TestClass
    {
        // Properties

        public string NonNullableReferenceProperty { get; set; }

        public string? NullableReferenceProperty { get; set; }
        public int ValueTypeProperty { get; set; }
        public int? NullableValueTypeProperty { get; set; }

        // Fields
        public string nonNullableField;
        public string? nullableField;
        public int valueTypeField;
        public int? nullableValueTypeField;

        // Methods with parameters
        public void MethodWithNonNullableParameter(string nonNullableParam) { }
        public void MethodWithNullableParameter(string? nullableParam) { }
        public void MethodWithValueTypeParameter(int valueTypeParam) { }
        public void MethodWithNullableValueTypeParameter(int? nullableValueTypeParam) { }

        // Events
        public event EventHandler NonNullableEvent;
        public event EventHandler? NullableEvent;
    }

#pragma warning restore S3459
    #endregion
}
