using System.Reflection;
using Snebur.Core.Converters;
using Microsoft.AspNetCore.Http;

namespace Snebur.Presentation.Common.Binders;

internal static class OperationParameterBinder
{
    internal static Result<object> BindParameter(
        HttpMethodDescriptor descriptor,
        HttpContext httpContext,
        ParameterInfo parameter,
        string parameterKey)
    {
        var parameterName = parameter.Name;

        if (string.IsNullOrWhiteSpace(parameterKey))
        {
            return Result.Failure<object>(new BadRequestError(
                "QueryKeyMissing",
                $"Error in method '{descriptor.Method.Name}' of '{descriptor.Method.DeclaringType?.Name}': " +
                "A query parameter is missing a name in the descriptor with query template " +
                $"'{descriptor.OperationTemplate}'."));
        }

        var parameterStringValue = GetParameterStringValue(
            httpContext.Request, 
            parameterKey);

        if(parameterStringValue is not null)
        {
            try
            {
                var convertedValue = OperatorParameterConverter.Parse(parameterStringValue, parameter);
                return Result.Success(convertedValue!);
            }
            catch (Exception ex)
            {
                return Result.Failure<object>(new BadRequestError(
                    "ParameterConversionFailed",
                    $"Error in method '{descriptor.Method.Name}' of '{descriptor.Method.DeclaringType?.Name}': " +
                    $"Failed to convert query parameter '{parameterName}'. Route {descriptor.RouteTemplate} (from query template '{descriptor.OperationTemplate}') " +
                    $"to type '{parameter.ParameterType.Name}'. Details: {ex.GetNestedMessage()}"));
            }
        }
      

        if (parameter.HasDefaultValue)
        {
            return Result.Success(parameter.DefaultValue!);
        }

        return Result.Failure<object>(new BadRequestError(
            "ParameterNotFound",
            $"Error in method '{descriptor.Method.Name}' of '{descriptor.Method.DeclaringType?.Name}': " +
            $"Missing required query parameter '{parameterName}' in query template '{descriptor.OperationTemplate}'."));
    }

    private static string? GetParameterStringValue(HttpRequest request, string key)
    {
        if (request.Query.TryGetValue(key, out var queryValue) && queryValue.Count > 0)
        {
            return  queryValue.ToString();
        }

        if (request.HasFormContentType && 
            request.Form.TryGetValue(key, out var formValue) && formValue.Count > 0)
        {
            return formValue.ToString();
        }
        return null;
    }
}
