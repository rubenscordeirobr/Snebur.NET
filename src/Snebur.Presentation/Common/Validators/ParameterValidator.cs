using System.Reflection;
using Snebur.Presentation.Common.Exceptions;

namespace Snebur.Presentation.Common.Validators;

internal static class ParameterValidator
{
    private static readonly Type[] SupportedTypes = {
            typeof(bool),
            typeof(byte),
            typeof(sbyte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(decimal),
            typeof(char),
            typeof(string),
            typeof(Guid),
            typeof(DateTime),
            typeof(TimeSpan)
        };

    internal static void Validate(HttpMethodDescriptor descriptor)
    {
        var parameters = descriptor.Parameters;
        if (parameters.Length == 0 || descriptor.IsBodyParameter)
        {
            return;
        }

        var routeParameters = descriptor.RouteParameters;
        var operatorParameters = descriptor.OperationParameters;
        var method = descriptor.Method;

        var validParameters = routeParameters
            .Concat(operatorParameters)
            .ToList();

        var missingParameters = parameters.Where(p => !p.HasDefaultValue && !validParameters.Contains(p)).ToList();
        if (missingParameters.Count > 0)
        {
            throw new HttpTemplateException(
                $"Error binding HTTP templates in method '{method.Name}' of type '{method.DeclaringType?.Name}': " +
                $"the following parameters are not mapped in the route or query template: {string.Join(", ", missingParameters)}. " +
                "Please ensure that every non-CancellationToken parameter in the method signature is referenced in the route and/or query template.");
        }

        if (operatorParameters.Length > 0 &&
            descriptor.HttpVerb != HttpVerb.Get &&
            descriptor.BodyContentType != BodyContentType.Form)
        {
            throw new HttpTemplateException(
                $"Error binding HTTP templates in method '{method.Name}' of type '{method.DeclaringType?.Name}': " +
                "the operation template is only supported for GET requests and form content types. " +
                "Please ensure that the operation template is only used for GET requests and form content types.");
        }

        if (operatorParameters.Length != descriptor.OperationParameterToKeyMap.Count)
        {
            throw new HttpTemplateException(
                $"Error binding HTTP templates in method '{method.Name}' of type '{method.DeclaringType?.Name}': " +
                $"the number of parameters in the operation template does not match the number of keys in the                 $\"the number of parameters in the operation template does not match the number of keys in the query template. \" +\r\n template. " +
                "Please ensure that each parameter is mapped to a key in the query template.");
        }

        var parameterDuplicate = routeParameters
            .Intersect(operatorParameters)
            .ToList();

        if (parameterDuplicate.Count > 0)
        {
            throw new DuplicateHttpTemplateException(
                $"Error binding HTTP templates in method '{method.Name}' of type '{method.DeclaringType?.Name}': " +
                $"the following parameters are mapped in both the route and query template: {string.Join(", ", parameterDuplicate)}. " +
                "Please ensure that each parameter is mapped to only one template.");
        }

        validParameters.ForEach(parameter => ValidateParameter(method, parameter));
    }

    private static void ValidateParameter(MethodInfo method, ParameterInfo paramerter)
    {
        ValidateParameterType(method, paramerter.ParameterType);
    }

    public static void ValidateParameterType(MethodInfo method, Type parameterType)
    {
        // Unwrap nullable types
        parameterType = parameterType.GetUnderlyingType();

        if (parameterType == typeof(CancellationToken))
        {
            return;
        }

        if (parameterType.IsPrimitive ||
            parameterType.IsEnum ||
            Array.Exists(SupportedTypes, t => t == parameterType))
        {
            return;
        }

        throw new HttpTemplateException(
            $"Error binding HTTP templates in method '{method.Name}' of type '{method.DeclaringType?.Name}': " +
            $"the parameter type '{parameterType.Name}' is not supported. " +
            "Please ensure that all parameters are one of the following types: " +
            $"{string.Join(", ", SupportedTypes.Select(t => t.Name))} " +
            "or an enum type.");
    }
}
