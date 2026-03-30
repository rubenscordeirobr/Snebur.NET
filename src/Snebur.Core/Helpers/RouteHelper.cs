using Snebur.Core.Utils;

namespace Snebur.Core.Helpers;

public static class RouteHelper
{
    public static string Combine(string route, string? otherRoute)
    {
        Guard.NotNull(route);

        route = route.Trim('/');

        var combined = string.IsNullOrWhiteSpace(otherRoute)
            ? route
            : $"{route}/{otherRoute.Trim('/')}";

        return $"/{combined}";
    }

    public static string CreateRoute(string methodName)
    {
        Guard.NotNullOrWhiteSpace(methodName);

        methodName = NormalizeMethodName(methodName);

        var kebabCase = CaseConventionUtils.ToKebabCase(methodName);
        return $"/{kebabCase}";
    }

    private static string NormalizeMethodName(string methodName)
    {
        if (methodName.EndsWith("Async"))
        {
            methodName = methodName[..^5];
        }

        if (methodName.StartsWith("Is"))
        {
            methodName = methodName[2..];
        }
        return methodName;
    }
}

