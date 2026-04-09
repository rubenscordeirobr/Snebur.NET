using System.Reflection;
using Snebur.Core.Extensions;

namespace Snebur.ClientGateway.Common.Helpers;

public static class RouteBinderHelper
{
    internal static string GetRoute<T>()
    {
        var type = typeof(T);
        var attribute = type.GetCustomAttribute<RouteAttribute>();
        if (attribute is null)
        {
            throw new InvalidOperationException($"Type '{type.Name}' does not have a RouteAttribute.");
        }
        return attribute.Route;
    }
 
    public static string? BindRoute(
        object obj,
        string? routeTemplate)
    {
        if (string.IsNullOrEmpty(routeTemplate))
        {
            return routeTemplate;
        }

        var binder = new RouteTemplateBinder(routeTemplate);
        return binder.Bind(obj);
    }

    internal static string? BindRoute(
       string[] parameterNames,
       object[] parameterValues,
       string? routeTemplate)
    {
        if (string.IsNullOrEmpty(routeTemplate))
        {
            return routeTemplate;
        }

        var binder = new RouteTemplateBinder(routeTemplate);
        return binder.Bind(parameterNames, parameterValues);
    }

    private sealed class RouteTemplateBinder
    {
        private readonly int _start;
        private readonly int _close;

        public string Route { get; }
        public string? ParameterName { get; }

        internal RouteTemplateBinder(string route)
        {
            Route = route;
            _start = route.IndexOf('{', StringComparison.Ordinal);
            _close = route.IndexOf('}', StringComparison.Ordinal);

            if (_start >= 0 && _close > _start)
            {
                ParameterName = route.Substring(_start + 1, _close - _start - 1);
            }
        }

        public string Bind(object obj)
        {
            if (ParameterName is null)
            {
                return Route;
            }

            Guard.NotNull(obj);
            
            var property = obj.GetType().GetPropertyByName(ParameterName);
            if (property is null)
            {
                throw new InvalidOperationException(
                    $"Property '{ParameterName}' for route template {Route} was not found in type '{obj.GetType().Name}'.");
            }
            try
            {
                var value = property.GetValue(obj);
                return BindValue(value);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Error binding property '{ParameterName}' for route template {Route} in type '{obj.GetType().Name}'.", ex);
            }
        }

        public string Bind(
            string[] parameterNames,
            object[] parameterValues)
        {
            if (ParameterName is null)
            {
                return Route;
            }
            var index = Array.IndexOf(parameterNames, ParameterName);
            if (index == -1)
            {
                throw new InvalidOperationException(
                    $"Parameter '{ParameterName}' for route template {Route} was not found in the parameter names.");
            }
            return BindValue(parameterValues[index]);
        }

        private string BindValue(object? value)
        {
            return $"{Route.Substring(0, _start)}{value}{Route.Substring(_close + 1)}";
        }
    }
}

