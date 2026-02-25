using System.Net;
using System.Reflection;
using Snebur.Core.Helpers;
using Snebur.Presentation.Common.Exceptions;
using Snebur.Presentation.Common.Validators;
using Snebur.SharedKernel.Abstractions;
using Microsoft.AspNetCore.Authorization;

namespace Snebur.Presentation.Common;

public sealed class HttpMethodDescriptor
{
    public HttpEndpointDescriptor HttpEndpointDescriptor { get; }
    public MethodInfo Method { get; }
    public HttpMethodAttribute Attribute { get; }
    public bool HasCancellationToken { get; }
    public bool IsAllowAnonymous { get; }
    public bool IsBodyParameter { get; }
    public string RouteTemplate { get; }
    public string OperationTemplate { get; }
    public Type ResponseType { get; }

    public ParameterInfo[] Parameters { get; }
    public ParameterInfo[] ParametersWithoutRoute { get; }
    public ParameterInfo[] RouteParameters { get; }
    public ParameterInfo[] OperationParameters { get; }

    public IReadOnlyDictionary<ParameterInfo, string> OperationParameterToKeyMap { get; }
    public BodyContentType BodyContentType { get; }
    public OperationParameterLocation OperationParameterLocation { get; }

    public HttpStatusCode SuccessStatusCode
        => Attribute.SuccessStatusCode;

    public HttpVerb HttpVerb
        => this.Attribute.HttpVerb;

    public Type? BodyType
        => IsBodyParameter
            ? Parameters.FirstOrDefault()?.ParameterType
            : null;

    internal HttpMethodDescriptor(
        HttpEndpointDescriptor endpointDescriptor,
        MethodInfo method)
    {
        var parameters = method.GetParameters();
        var lastParameter = parameters.LastOrDefault();

        HttpEndpointDescriptor = endpointDescriptor;
        Method = method;
        Attribute = method.GetCustomAttribute<HttpMethodAttribute>()
            ?? throw new HttpTemplateException("Method must have an HttpMethodAttribute.");

        HasCancellationToken = lastParameter is not null
            && lastParameter.ParameterType == typeof(CancellationToken);

        IsAllowAnonymous = HasAllowAnonymousAttribute();

        Parameters = HasCancellationToken
            ? parameters[..^1]
            : parameters;

        RouteTemplate = GetRouteTemplate();
        RouteParameters = GetRouteParameters();
        ParametersWithoutRoute = [.. Parameters.Except(RouteParameters)];

        IsBodyParameter = IsBodyParameterPresent();
        OperationParameters = GetOperationParameters();
        OperationTemplate = GetOperationTemplate();
        OperationParameterToKeyMap = MapOperationParametersToKeys();
        ResponseType = ResolveResponseType(Method.ReturnType);
        BodyContentType = ResolveBodyContentType();

        OperationParameterLocation = BodyContentType == BodyContentType.Form
            ? OperationParameterLocation.BodyForm
            : OperationParameterLocation.Query;

        ParameterValidator.Validate(this);
    }

    private bool HasAllowAnonymousAttribute()
    {
        return Method.GetCustomAttribute<AllowAnonymousAttribute>() is not null ||
            Method.DeclaringType?.GetCustomAttribute<AllowAnonymousAttribute>() is not null;
    }

    private bool IsBodyParameterPresent()
    {
        if (ParametersWithoutRoute.Length == 1)
        {
            var parameterType = ParametersWithoutRoute[0].ParameterType;
            return parameterType.ImplementsGenericInterfaceDefinition(typeof(IRequest<>));
        }
        return false;
    }

    private string GetRouteTemplate()
    {
        if (string.IsNullOrWhiteSpace(Attribute.RouteTemplate) &&
            HttpEndpointDescriptor.ShouldGenerateRoute(Method))
        {
            return RouteHelper.CreateRoute(Method.Name);
        }
        return Attribute.RouteTemplate;
    }

    private string GetOperationTemplate()
    {
        var operationTemplate = Attribute.OperationTemplate;
        var needsCreateOperatorTemplate = string.IsNullOrEmpty(operationTemplate) &&
            Parameters.Length > 0 &&
            !IsBodyParameter;

        if (needsCreateOperatorTemplate)
            return CreateOperatorTemplate();

        return operationTemplate;
    }

    private string CreateOperatorTemplate()
    {
        var queryParameters = OperationParameters
            .Select(p => OperationParameterUtils.NormalizeKey(p.Name))
            .Select(key => $"{key}={{{key}}}");

        return string.Join("&", queryParameters);
    }

    private ParameterInfo[] GetRouteParameters()
    {
        if (Parameters.Length == 0)
            return [];

        var routeParameters = new List<ParameterInfo>();
        var routeParts = RouteTemplate.Split('/', StringSplitOptions.RemoveEmptyEntries);

        foreach (var routePart in routeParts)
        {
            if (routePart.Contains('{', StringComparison.Ordinal) &&
                routePart.Contains('}', StringComparison.Ordinal))
            {
                var parameterName = ExtractParameterName(routePart);
                var parameter = Parameters.FirstOrDefault(x => x.Name == parameterName);
                if (parameter is null)
                {
                    throw new RouteTemplateException(
                        $"Error binding route template '{RouteTemplate}' in method '{Method.Name}' of type '{Method.DeclaringType?.Name}': " +
                        $"the route segment '{{{parameterName}}}' does not match any parameter in the method signature. " +
                        "Please verify that the route template references an existing parameter.");
                }
                routeParameters.Add(parameter);
            }
        }
        return [.. routeParameters];
    }

    private string? ExtractParameterName(string routePart)
    {
        var start = routePart.IndexOf('{', StringComparison.Ordinal);
        var end = routePart.IndexOf('}', StringComparison.Ordinal);
        if (end > start)
        {
            return routePart.Substring(start + 1, end - start - 1).TrimStart('*');
        }

        throw new RouteTemplateException(
            $"Error binding route template '{RouteTemplate}' in method '{Method.Name}' of type '{Method.DeclaringType?.Name}': " +
            $"the route segment '{routePart}' is invalid. It must be in the format '{{parameterName}}'. " +
            "Please review the route template syntax.");
    }

    private ParameterInfo[] GetOperationParameters()
    {
        if (IsBodyParameter)
        {
            return [];
        }
        return ParametersWithoutRoute;
    }

    private Dictionary<ParameterInfo, string> MapOperationParametersToKeys()
    {
        if (Parameters.Length == 0)
        {
            return new Dictionary<ParameterInfo, string>();
        }

        var mappings = new Dictionary<ParameterInfo, string>();
        var pairs = OperationTemplate.Split('&', StringSplitOptions.RemoveEmptyEntries);

        foreach (var pair in pairs)
        {
            var keyValue = pair.Split('=', StringSplitOptions.RemoveEmptyEntries);
            if (keyValue.Length != 2)
            {
                throw new OperationTemplateException(
                    $"Error binding operation template '{OperationTemplate}' in method '{Method.Name}' of type '{Method.DeclaringType?.Name}': " +
                    $"the key-value pair '{pair}' is invalid. It must be in the format 'key={{parameterName}}'. " +
                    "Please review the query template syntax.");
            }

            var queryKey = keyValue[0];
            var value = keyValue[1];
            if (value.StartsWith('{') && value.EndsWith('}'))
            {
                var parameterName = value.Substring(1, value.Length - 2);
                var parameter = OperationParameters.FirstOrDefault(x => x.Name == parameterName);
                if (parameter is null)
                {
                    throw new OperationTemplateException(
                        $"Error binding operation template '{OperationTemplate}' in method '{Method.Name}' of type '{Method.DeclaringType?.Name}': " +
                        $"the parameter '{parameterName}' specified in the template is not present in the method signature. " +
                        "Ensure that all parameters referenced in the query template are defined in the method.");
                }
                mappings[parameter] = queryKey;
            }
        }
        return mappings;
    }

    private Type ResolveResponseType(Type currentType)
    {
        if (currentType.IsGenericType)
        {
            if (currentType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                return ResolveResponseType(currentType.GenericTypeArguments[0]);
            }

            if (currentType.GetGenericTypeDefinition() == typeof(Result<>))
            {
                return ResolveResponseType(currentType.GenericTypeArguments[0]);
            }
        }
        return currentType;
    }

    private BodyContentType ResolveBodyContentType()
    {
        if (HttpVerb == HttpVerb.Get || ParametersWithoutRoute.Length == 0)
        {
            return BodyContentType.None;
        }

        if (IsBodyParameter)
        {
            return BodyContentType.Json;
        }

        if (OperationParameters.Length > 0)
        {
            return BodyContentType.Form;
        }

        throw new HttpTemplateException("Failed to resolve the body content type.");
    }
}
