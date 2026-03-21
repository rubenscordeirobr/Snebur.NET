using System.Linq.Expressions;

namespace Snebur.Core.Factories;

public static class ErrorCodeFactory
{

    public static string CreateCodeFor<T>(Expression<Func<T, object>> memberExpression)
    {
        var memberName = memberExpression.GetMemberPath();
        return CreateCodeFor<T>(memberName);
    }
    public static string CreateCodeFor<T>(string memberName)
    {
        return CreateCodeFor(typeof(T), memberName);
    }

    public static string CreateCodeFor(Type type, string memberName)
    {
        Guard.NotNull(type);
        return $"{type.Name}.{memberName}";
    }

    public static string CreateInvalidCodeFor<T>(Expression<Func<T, object>> propertyExpression)
    {
        var propertyName = propertyExpression.GetMemberPath();
        return CreateInvalidCodeFor<T>(propertyName);
    }
    public static string CreateInvalidCodeFor<T>(string propertyName)
    {
        return CreateInvalidCodeFor(typeof(T), propertyName);
    }

    public static string CreateInvalidCodeFor(Type type, string propertyName)
    {
        Guard.NotNull(type);
        return $"{type.Name}.{propertyName}Invalid";
    }

    public static string CommandValidatorFoundCodeFor(string commandTypeName)
    {
        return $"{commandTypeName}.ValidatorNotFound";
    }
}
