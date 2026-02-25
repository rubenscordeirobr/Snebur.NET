namespace Snebur.Core.UnitTests.Extensions;
public class PropertyInfoExtensionsTests
{
    private class NestedPropertyTestClass
    {
        public string? TestProperty { get; set; }
    }

    [Theory]
    [InlineData(typeof(NestedPropertyTestClass), nameof(NestedPropertyTestClass.TestProperty), "Snebur.Core.UnitTests.Extensions.PropertyInfoExtensionsTests+NestedPropertyTestClass::TestProperty")]
    [InlineData(typeof(TestPropertyPathClass), nameof(TestPropertyPathClass.TestProperty), "Snebur.Core.UnitTests.Extensions.TestPropertyPathClass::TestProperty")]
    public void GetPropertyPath_ShouldReturnCorrectPath(Type type, string propertyName, string result)
    {
        // Arrange
        var propertyInfo = type.GetProperty(propertyName);

        // Act
        var propertyPath = propertyInfo?.GetPropertyPath();

        // Assert
        propertyPath.Should().Be(result);
    }
}

internal class TestPropertyPathClass
{
    public string? TestProperty { get; set; }
}
