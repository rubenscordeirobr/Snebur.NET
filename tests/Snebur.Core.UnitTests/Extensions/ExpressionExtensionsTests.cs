using System.Linq.Expressions;

namespace Snebur.Core.UnitTests.Extensions;

public class ExpressionExtensionsTests
{
    [Fact]
    public void GetMemberName_ShouldReturnMemberName_WhenExpressionIsValid()
    {
        // Arrange
        Expression<Func<TestClass, object>> expression = x => x.TestProperty!;

        // Act
        var memberName = expression.GetMemberName();

        // Assert
        memberName.Should().Be(nameof(TestClass.TestProperty));
    }

    [Fact]
    public void GetMemberName_ShouldThrowArgumentException_WhenExpressionIsNotMemberExpression()
    {
        // Arrange
        Expression<Func<TestClass, object>> expression = x => x.ToString()!;

        // Act
        Action act = () => expression.GetMemberName();

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("The LambdaExpression 'x => x.ToString()' is not a member expression");
    }

    [Fact]
    public void GetMemberPath_ShouldReturnMemberPath_WhenExpressionIsValid()
    {
        // Arrange
        Expression<Func<TestClass, object>> expression = x => x.TestProperty!;

        // Act
        var memberPath = expression.GetMemberPath();

        // Assert
        memberPath.Should().Be(nameof(TestClass.TestProperty));
    }

    [Fact]
    public void GetMemberPath_ShouldReturnNestedMemberPath_WhenExpressionIsValid()
    {
        // Arrange
        Expression<Func<TestClass, object>> expression = x => x.Nested.TestProperty!;
                               
        // Act
        var memberPath = expression.GetMemberPath();

        // Assert
        memberPath.Should().Be("Nested.TestProperty");
    }

    [Fact]
    public void GetMemberPath_ShouldThrowArgumentException_WhenExpressionIsNotMemberExpression()
    {
        // Arrange
        Expression<Func<TestClass, object>> expression = x => x.ToString()!;

        // Act
        Action act = () => expression.GetMemberPath();

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("The LambdaExpression 'x => x.ToString()' is not a member expression");
    }

    private class TestClass
    {
        public string? TestProperty { get; set; }
        public NestedClass Nested { get; set; } = new NestedClass();
    }

    private class NestedClass
    {
        public string? TestProperty { get; set; }
    }
}

