using System.Text.RegularExpressions;

namespace Snebur.UI.Helpers;

public static partial class BlazorRouteHelper
{
    [GeneratedRegex($@"(/)?\{{\s*\w+\s*\}}")]
    private static partial Regex FirstParameterRegex();

    public static string GetRoute<TComponent>()
    {
        return GetRoute(typeof(TComponent));
    }

    public static string GetRoute(Type componentType)
    {
        Guard.NotNull(componentType);

        return componentType
            .GetCustomAttributes(typeof(RouteAttribute), inherit: false)
            .Cast<RouteAttribute>()
            .Select(attr => RemoveFirstParameter(attr.Template))
            .FirstOrDefault() ??
            throw new MissingAttributeException(nameof(RouteAttribute), componentType.Name);
                
    }

    public static string[] GetRoutes<TComponent>()
    {
        return GetRoutes(typeof(TComponent));
    }

    public static string[] GetRoutes(Type componentType)
    {
        Guard.NotNull(componentType);

        return componentType
            .GetCustomAttributes(typeof(RouteAttribute), inherit: false)
            .Cast<RouteAttribute>()
            .Select(attr => RemoveFirstParameter(attr.Template))
            .ToArray();
    }

    private static string RemoveFirstParameter(string template)
    {
        if (string.IsNullOrWhiteSpace(template))
            return template;

        var match = FirstParameterRegex().Match(template);
        if (match.Success)
        {
            var startIndex = match.Index;
            var endIndex = match.Index + match.Length;
            return template.Remove(startIndex, endIndex - startIndex);
        }
        return template;
    }
}
