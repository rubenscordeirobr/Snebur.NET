using System.Linq.Expressions;
using System.Text;

namespace Snebur.Core.Extensions;

public static class ExpressionExtensions
{
    public static string GetMemberName<T>(this Expression<Func<T, object>> expression)
    {
        Guard.NotNull(expression);

        var memberExpression = GetMemberExpression(expression);
        return memberExpression.Member.Name;
    }

    public static string GetMemberPath<T>(this Expression<Func<T, object>> expression)
    {
        Guard.NotNull(expression);

        var memberExpression = GetMemberExpression(expression);

        var propertyPath = new StringBuilder();
        while (memberExpression != null)
        {
            if (propertyPath.Length > 0)
                propertyPath.Insert(0, ".");

            propertyPath.Insert(0, memberExpression.Member.Name);
            memberExpression = memberExpression.Expression as MemberExpression;
        }

        return propertyPath.ToString();
    }

    private static MemberExpression GetMemberExpression(LambdaExpression expression)
    {
        var expressionBody = expression.Body;
        if (expressionBody is MemberExpression memberExpression)
            return memberExpression;

        if (expressionBody is UnaryExpression unaryExpression && unaryExpression.Operand is MemberExpression operand)
            return operand;

        throw new ArgumentException($"The LambdaExpression '{expression}' is not a member expression");
    }
}

