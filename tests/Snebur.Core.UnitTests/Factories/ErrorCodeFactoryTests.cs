using System.Linq.Expressions;
using Snebur.Core.Factories;

namespace Snebur.Core.UnitTests.Factories;

public class ErrorCodeFactoryTests
{
    [Theory]
    [InlineData(typeof(SampleClass), "Property1", "SampleClass.Property1Invalid")]
    [InlineData(typeof(SampleClass), "Property2", "SampleClass.Property2Invalid")]
    public void CreateInvalidCodeFor_ShouldReturnExpectedCode(Type type, string propertyName, string expected)
    {
        var result = ErrorCodeFactory.CreateInvalidCodeFor(type, propertyName);
        result.Should().Be(expected);
    }

    [Fact]
    public void CreateInvalidCodeFor_WithExpression_ShouldReturnExpectedCode()
    {
        Expression<Func<SampleClass, object>> expression = x => x.Property1!;
        var result = ErrorCodeFactory.CreateInvalidCodeFor(expression);
        result.Should().Be("SampleClass.Property1Invalid");
    }
     
    private class SampleClass
    {
        public string? Property1 { get; set; }
        public string? Property2 { get; set; }
    }
}

