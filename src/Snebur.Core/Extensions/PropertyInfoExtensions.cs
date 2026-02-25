using System.Reflection;

namespace Snebur.Core.Extensions;

public static class PropertyInfoExtensions
{
    public static string GetPropertyPath(this PropertyInfo propertyInfo)
    {
        Guard.NotNull(propertyInfo);

        return $"{propertyInfo.DeclaringType?.GetQualifiedName()}::{propertyInfo.Name}";
    }
}
