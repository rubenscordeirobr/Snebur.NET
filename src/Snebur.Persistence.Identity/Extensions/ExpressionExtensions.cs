using System.Reflection;
using FluentValidation.Internal;

namespace Snebur.Persistence.Identity.Extensions;

internal static class ExpressionExtensions
{
    internal static void SetPropertyValue<T, TProperty>(this T user,
       Expression<Func<T, TProperty>> expression, TProperty value)
    {
        var memberInfo = expression.GetMember()
            ?? throw new InvalidOperationException("Invalid member expression.");

        if (memberInfo is not PropertyInfo propertyInfo)
        {
            throw new InvalidOperationException("Member is not a property.");
        }

        if (propertyInfo.SetMethod == null)
        {
            throw new InvalidOperationException("Property is read-only.");
        }
        propertyInfo.SetValue(user, value);
    }
}
